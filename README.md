"# Dapr" 

    dapr run --app-id palprimesapi --app-port 5296 --dapr-http-port 3500 dotnet run
    dapr run --app-id palapi --app-port 5221 --dapr-http-port 3510 dotnet run
    dapr run --app-id primespi --app-port 5225 --dapr-http-port 3520 dotnet run



- Frontend app, med signalR
- Implementere algoritmer for Prime og Pal
- Fix bug med Prime-api
- compeating consumers - skalering ("consumer.groups in redis")
- 