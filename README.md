"# Dapr" 

    dapr run --app-id palprimesapi --app-port 5296 --dapr-http-port 3500 dotnet run
    dapr run --app-id palapi --app-port 5221 --dapr-http-port 3510 dotnet run
    dapr run --app-id primespi --app-port 5225 --dapr-http-port 3520 dotnet run

1. Docerize
1. Frontend app, med signalR
1. Implementere algoritmer for Prime og Pal
1. Fix bug med Prime-api (DONE)
1. Compeating consumers - skalering ("consumer.groups in redis")
1. Docker-Desktop Kubernetes
1. Azure Kubernetes

## Docker

Run from src folder.

    docker-compose -f "docker-compose.yml" up -d --build