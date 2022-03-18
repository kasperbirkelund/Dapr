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
1. Docker-Desktop Kubernetes
1. Get zipkin to work on Docker
1. Compeating consumers - skalering ("consumer.groups in redis")
1. Azure Kubernetes

## Docker

Run all commands from root folder.

### Compose up

    docker-compose.exe -f .\docker-compose.debug.yml up -d --build

### Compose down

    docker-compose.exe -f .\docker-compose.debug.yml down

## Angular app

> Start backend in Docker first.

CD to Frontend/Angular/palprimes-app folder and run the command below.

    ng serve

Hit <http://localhost:4200> in your browser.