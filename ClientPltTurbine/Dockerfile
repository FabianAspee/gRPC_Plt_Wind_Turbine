#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 5001 

FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build 
WORKDIR /src
COPY ["ClientPltTurbine/ClientPltTurbine.csproj", "ClientPltTurbine/"]
RUN dotnet restore "ClientPltTurbine/ClientPltTurbine.csproj"
COPY . .
WORKDIR "/src/ClientPltTurbine"
RUN dotnet build "ClientPltTurbine.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ClientPltTurbine.csproj" -c Release -o /app/publish

FROM base AS final
ENV ASPNETCORE_URLS="https://+;http://+"
ENV ASPNETCORE_HTTPS_PORT=443 
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="" 
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/pltcertificate.pfx 
WORKDIR /

COPY ["ClientPltTurbine/pltcertificate.pfx", "/https/pltcertificate.pfx"]
ADD ClientPltTurbine/pltcertificate.pfx :/container/cert/path
RUN update-ca-certificates
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["ClientPltTurbine/App.config", "/app/config/App.config"] 
COPY ["ClientPltTurbine/wwwroot/images/", "/app/wwwroot/images/"] 
ENTRYPOINT ["dotnet", "ClientPltTurbine.dll"]