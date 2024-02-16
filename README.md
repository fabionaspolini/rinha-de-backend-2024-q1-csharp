# Rinha de backend 2024/Q1

## AOT publish

Exemplo: https://github.com/dotnet/samples/blob/main/core/nativeaot/HelloWorld/Dockerfile

```bash
dotnet publish src/RinhaBackend-2024-q1.csproj -c Release -r win-x64 -o out
dotnet publish src/RinhaBackend-2024-q1.csproj -c Release -r linux-x64 -o out
```

## Docker

```bash
docker build -t rinha-api .

docker run --rm \
    --name rinha \
    --network postgres \
    -p 9999:9999 \
    -p 5200:5200 \
    -e "ASPNETCORE_URLS=http://*:5200" \
    -e "ConnectionStrings__Rinha=Server=postgres-16;Port=5432;Database=rinha-de-backend-2024-q1;User Id=postgres;Password=123456;" \
    rinha-api

docker run --rm -it --name rinha -p 9999:9999 rinha-api bash
```
