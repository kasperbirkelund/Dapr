# Dapr

    dapr run --app-id palprimesapi --app-port 5296 --dapr-http-port 3500 dotnet run
    dapr run --app-id palapi --app-port 5221 --dapr-http-port 3510 dotnet run
    dapr run --app-id primespi --app-port 5225 --dapr-http-port 3520 dotnet run

## Tasks

1. Dockerize (DONE)
1. Frontend app, 
    1. Blazor
    1. SignalR (DONE)
    1. Angular (DONE)
1. Implementere algoritmer for Prime og Pal (DONE)
1. Fix bug med Prime-api (DONE)
1. Docker-Desktop Kubernetes (DONE)
        1. Add SignalR back plane.
1. Get zipkin to work on Docker and Kubernetes
1. Competing consumers - skalering ("consumer.groups in redis")
1. Azure Kubernetes

## Run apps on Docker

Run all commands from root folder.

### Compose up

    docker-compose.exe -f .\docker-compose.yml -f .\docker-compose.debug.yml up -d --build

Hit <http://localhost:5220> in your browser to go to angular frontend.

### Compose down

    docker-compose.exe -f .\docker-compose.yml -f .\docker-compose.debug.yml down

## Run Angular app locally

In case you want to run angular frontend outside Docker than start backend in Docker first and run ng serve.

CD to Frontend/Angular/angular-app folder and run the command below.

    ng serve

Hit <http://localhost:4200> in your browser.

## Run apps on Kubernetes

>Before you continue make sure to setup Kubernetes and Dapr on [Kubernetes](#Kubernetes).

### Build & package apps

1. Run docker compose in case you need new build

        docker-compose.exe -f .\docker-compose.yml up -d --build

1. Run below commands to create helm package for each app

        helm package --version "0.0.0-latest" --app-version latest --destination ./dist/helm ./src/Frontend/Angular/angular-app/charts
        helm package --version "0.0.0-latest" --app-version latest --destination ./dist/helm ./src/Palprimes.Api/Charts
        helm package --version "0.0.0-latest" --app-version latest --destination ./dist/helm ./src/Pal.Api/Charts
        helm package --version "0.0.0-latest" --app-version latest --destination ./dist/helm ./src/Prime.Api/Charts

1. Run below commands to deploy all apps

    > In case you want test the output just uncomment ```--dry-run >> ./dist/{appname}.yaml```.

        helm upgrade angularapp -f ./src/Frontend/Angular/angular-app/charts/values.yaml --install --recreate-pods --version=latest -n palprimes --set buildID=latest --set image.tag=latest ./dist/helm/angularapp-0.0.0-latest.tgz # --dry-run >> ./dist/angularapp.yaml

        helm upgrade palprimesapi -f ./src/Palprimes.Api/Charts/values.yaml --install --recreate-pods --version=latest -n palprimes --set buildID=latest --set image.tag=latest --set ASPNETCORE_ENVIRONMENT=Production ./dist/helm/palprimesapi-0.0.0-latest.tgz #--dry-run >> dist/palprimesapi.yaml

        helm upgrade primeapi -f ./src/Prime.Api/Charts/values.yaml --install --recreate-pods --version=latest -n palprimes --set buildID=latest --set image.tag=latest --set ASPNETCORE_ENVIRONMENT=Production ./dist/helm/primeapi-0.0.0-latest.tgz #--dry-run >> dist/primeapi.yaml

        helm upgrade palapi -f ./src/Pal.Api/Charts/values.yaml --install --recreate-pods --version=latest -n palprimes --set buildID=latest --set image.tag=latest --set ASPNETCORE_ENVIRONMENT=Production ./dist/helm/palapi-0.0.0-latest.tgz #--dry-run >> dist/palapi.yaml

### Uninstall apps

        helm uninstall angularapp --namespace palprimes
        helm uninstall palprimesapi --namespace palprimes
        helm uninstall palapi --namespace palprimes
        helm uninstall primeapi --namespace palprimes

## Kubernetes

>Required only the first time.

### Enable Kubernetes on Docker Desktop 

![Kubernetes on Docker Desktop](images/docker-desktop-k8s.png)

### Install Kubernetes Metrics Server

Metrics yaml fetched from <https://github.com/kubernetes-sigs/metrics-server/releases>

Inspiration from <https://dev.to/docker/enable-kubernetes-metrics-server-on-docker-desktop-5434>

        kubectl apply -f .\kubernetes\metrics\components.yaml

### Install Kubernetes Dashboard (optional)

Good in case you want visuals of Kubernetes cluster with some basic administration.

1. Install dashboard

        kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.5.0/aio/deploy/recommended.yaml

1. Disable auth (only on local machine for convenience)

        kubectl patch deployment kubernetes-dashboard -n kubernetes-dashboard --type 'json' -p '[{"op": "add", "path": "/spec/template/spec/containers/0/args/-", "value": "--enable-skip-login"}]'

1. Access dashboard

        kubectl proxy

Dashboard can be accessed at: <http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/#/overview?namespace=default>

### Install Dapr

Inspiration taken from her: <https://docs.dapr.io/operations/hosting/kubernetes/kubernetes-deploy/>
>Note Dapr has been installed using helm charts.

1. Add dapr helm repo

        helm repo add dapr https://dapr.github.io/helm-charts/
        helm repo update

1. See which chart versions are available
    
        helm search repo dapr --devel --versions

1. Install dapr

        helm upgrade --install dapr dapr/dapr --version=1.6 --namespace dapr-system --create-namespace --wait

    >Uninstall dapr by running ```helm uninstall dapr --namespace dapr-system``` 

1. Verify dapr installation

        kubectl get pods --namespace dapr-system

        NAME                                    READY   STATUS    RESTARTS   AGE
        dapr-dashboard-8664d5c45f-4nxpw         1/1     Running   0          2m19s
        dapr-operator-58b9d5fd59-x44mr          1/1     Running   0          2m19s
        dapr-placement-server-0                 1/1     Running   0          2m19s
        dapr-sentry-858fddc4f7-9hp2w            1/1     Running   0          2m19s
        dapr-sidecar-injector-7497b7945-b9g5g   1/1     Running   0          2m19s

### Install Redis & Configure pubsub and statestore

1. Add bitnami repo 

        helm repo add bitnami https://charts.bitnami.com/bitnami
        helm repo update

1. Create palprimes namespace

        kubectl create namespace palprimes

1. Install Redis

        helm install redis bitnami/redis --namespace palprimes --set auth.password=gexo1!

1. Verify Redis installation

        kubectl get pods --namespace palprimes

        NAME               READY   STATUS    RESTARTS   AGE
        redis-master-0     1/1     Running   0          64s
        redis-replicas-0   1/1     Running   0          64s
        redis-replicas-1   0/1     Running   0          22s

    >Uninstall Redis by running ```helm uninstall redis --namespace palprimes```

1. Configure pubsub and statestore

        kubectl apply -f ./kubernetes/components/redis-statestore.yaml
        kubectl apply -f ./kubernetes/components/redis-pubsub.yaml

### Deploy and Configure Zipkin

1. Create Zipkin Deployment

        kubectl create deployment zipkin --image openzipkin/zipkin --namespace dapr-system

1. Create a Kubernetes service for the Zipkin pod

        kubectl expose deployment zipkin --type ClusterIP --port 9411 --namespace dapr-system

1. Deploy Dapr config

        kubectl apply -f ./kubernetes/config/tracing.yaml

### Access Zipkin

Expose Zipkin

        kubectl port-forward svc/zipkin 9411:9411 --namespace dapr-system

Access zipkin at: <http://localhost:9411>

### Access Dapr dashboard

1. Expose dashboard

        dapr dashboard -k

1. Access dashboard at <http://localhost:8080>


