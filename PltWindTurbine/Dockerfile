#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 5001 

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PltWindTurbine/PltWindTurbine.csproj", "PltWindTurbine/"]
RUN dotnet restore "PltWindTurbine/PltWindTurbine.csproj"
COPY . .
WORKDIR "/src/PltWindTurbine"
RUN dotnet build "PltWindTurbine.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PltWindTurbine.csproj" -c Release -o /app/publish

FROM base AS final 
ENV ASPNETCORE_URLS="https://+;http://+"
ENV ASPNETCORE_HTTPS_PORT=443 
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="8620rib53" 
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/pltcertificate.pfx 
WORKDIR /
COPY ["PltWindTurbine/pltcertificate.pfx", "/https/pltcertificate.pfx"]

WORKDIR /app
COPY --from=publish /app/publish . 
COPY ["PltWindTurbine/App.config", "/app/config/App.config"] 
COPY ["PltWindTurbine/Resources/*", "/app/Resources/*"] 
ENTRYPOINT ["dotnet", "PltWindTurbine.dll"]