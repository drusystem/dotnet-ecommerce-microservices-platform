# E-Commerce Microservices Platform

## Arquitectura
- **API Gateway**: YARP Reverse Proxy (Puerto 5000)
- **Catalog Service**: Gestión de productos (Puerto 5001)
- **Basket Service**: Carrito de compras (Puerto 5002)
- **Ordering Service**: Gestión de pedidos (Puerto 5003)
- **Identity Service**: Autenticación y autorización (Puerto 5004)

## Stack Tecnológico
- .NET 8
- Docker & Docker Compose
- YARP (API Gateway)
- PostgreSQL / SQL Server
- Redis
- RabbitMQ

## Estado del Proyecto
- [x] Etapa 1: Estructura base y API Gateway
- [ ] Etapa 2: Catalog Service
- [ ] Etapa 3: Basket Service
- [ ] Etapa 4: Ordering Service
- [ ] Etapa 5: Identity Service
- [ ] Etapa 6: Event-Driven Communication
- [ ] Etapa 7: Docker Compose & Deployment

## Ejecutar el proyecto
```bash
# Ejecutar API Gateway
cd src/ApiGateway/ApiGateway
dotnet run
```