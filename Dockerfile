# Build project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY . ./
WORKDIR /app/src
RUN dotnet restore -r linux-x64
RUN dotnet publish RinhaBackend-2024-q1.csproj -c Release -r linux-x64 -o out --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_URLS http://*:9999
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "RinhaBackend-2024-q1.dll"]