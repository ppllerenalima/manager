# Imagen base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Imagen para compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar csproj y restaurar dependencias
COPY src/Manager.API/Manager.API.csproj src/Manager.API/
RUN dotnet restore src/Manager.API/Manager.API.csproj

# Copiar el resto del código
COPY . .

WORKDIR /src/src/Manager.API
RUN dotnet build "Manager.API.csproj" -c Release -o /app/build

# Publicación
FROM build AS publish
RUN dotnet publish "Manager.API.csproj" -c Release -o /app/publish

# Imagen final para ejecución
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Manager.API.dll"]
