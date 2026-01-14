FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar csproj y restaurar (Gateway tiene doble carpeta también)
COPY ["src/ApiGateway/ApiGateway/ApiGateway.csproj", "ApiGateway/ApiGateway/"]
RUN dotnet restore "ApiGateway/ApiGateway/ApiGateway.csproj"

# Copiar código fuente
COPY src/ApiGateway/ ApiGateway/

# Build
WORKDIR "/src/ApiGateway/ApiGateway"
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Docker

ENTRYPOINT ["dotnet", "ApiGateway.dll"]