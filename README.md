# Rinha de backend 2024/Q1

## AOT publish

Exemplo: https://github.com/dotnet/samples/blob/main/core/nativeaot/HelloWorld/Dockerfile

```bash
dotnet publish src/RinhaBackend-2024-q1.csproj -c Release -r win-x64 -o out
dotnet publish src/RinhaBackend-2024-q1.csproj -c Release -r linux-x64 -o out
```

## Docker build

```bash
docker build --build-arg="ASYNC_METHODS=true" -t fabionaspolini/rinha-backend-2024-q1-aot-dapper:async .
docker build --build-arg="ASYNC_METHODS=false" -t fabionaspolini/rinha-backend-2024-q1-aot-dapper:sync .

docker run --rm \
    --name rinha \
    --network postgres \
    -p 9999:9999 \
    -e "ASPNETCORE_URLS=http://*:9999" \
    -e "ConnectionStrings__Rinha=Server=postgres-16;Port=5432;Database=rinha-de-backend-2024-q1;User Id=postgres;Password=123456;" \
    fabionaspolini/rinha-backend-2024-q1-aot-dapper:async

docker run --rm -it --name rinha -p 9999:9999 fabionaspolini/rinha-backend-2024-q1-aot-dapper:async bash
```

## Docker compose

```bash
docker compose -f "docker-compose-async.yml" up
docker compose -f "docker-compose-async.yml" down -v


docker compose -f "docker-compose-sync.yml" up
docker compose -f "docker-compose-sync.yml" down -v
```