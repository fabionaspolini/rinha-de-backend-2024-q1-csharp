# Build project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Install NativeAOT build prerequisites
RUN apt-get update && \
    apt-get install -y --no-install-recommends clang zlib1g-dev

WORKDIR /app
COPY ./RinhaBackend-2024-q1-aot-dapper.csproj ./
RUN dotnet restore -r linux-x64

COPY . ./

ARG ASYNC_METHODS=true
RUN dotnet publish RinhaBackend-2024-q1-aot-dapper.csproj \
    -c Release \
    -r linux-x64 \
    -o out \
    --no-restore \
    -p AsyncMethods=${ASYNC_METHODS}

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0
# FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine3.19
ENV ASPNETCORE_URLS http://*:9999
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["./RinhaBackend-2024-q1-aot-dapper"]