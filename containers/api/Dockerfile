# Dockerfile corregido para .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /project

COPY ["src/Manager.API/Manager.API.csproj", "src/Manager.API/"]
COPY . .
WORKDIR "/project/src/Manager.API"
RUN dotnet build "Manager.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Manager.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Manager.API.dll"]
