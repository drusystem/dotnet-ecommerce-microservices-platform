FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar csproj y restaurar (considerando la estructura real con doble carpeta)
COPY ["src/Services/Basket/Basket.API/Basket.API/Basket.API.csproj", "Services/Basket/Basket.API/Basket.API/"]
COPY ["src/Services/Basket/Basket.Application/Basket.Application.csproj", "Services/Basket/Basket.Application/"]
COPY ["src/Services/Basket/Basket.Domain/Basket.Domain.csproj", "Services/Basket/Basket.Domain/"]
COPY ["src/Services/Basket/Basket.Infrastructure/Basket.Infrastructure.csproj", "Services/Basket/Basket.Infrastructure/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Common/BuildingBlocks.Common.csproj", "BuildingBlocks/BuildingBlocks.Common/"]

RUN dotnet restore "Services/Basket/Basket.API/Basket.API/Basket.API.csproj"

# Copiar todo el código fuente
COPY src/Services/Basket/ Services/Basket/
COPY src/BuildingBlocks/ BuildingBlocks/

# Build
WORKDIR "/src/Services/Basket/Basket.API/Basket.API"
RUN dotnet build "Basket.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Basket.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Docker

ENTRYPOINT ["dotnet", "Basket.API.dll"]