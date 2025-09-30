# Dockerfile corregido para .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Instala la biblioteca para soporte NTLM/Kerberos
RUN apt-get update && apt-get install -y libgssapi-krb5-2 gss-ntlmssp && apt-get clean

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /project

COPY ["src/Manager.Reporting.Api/Manager.Reporting.Api.csproj", "src/Manager.Reporting.Api/"]
COPY . .
WORKDIR "/project/src/Manager.Reporting.Api"
RUN dotnet build "Manager.Reporting.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Manager.Reporting.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Manager.Reporting.Api.dll"]

