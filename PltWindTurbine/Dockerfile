#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
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
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="test123" 
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx 
WORKDIR /

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PltWindTurbine.dll"]