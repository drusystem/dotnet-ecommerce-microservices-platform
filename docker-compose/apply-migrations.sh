#!/bin/bash

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${CYAN}========================================"
echo -e "  Aplicando Migraciones - EcommerceMS  "
echo -e "========================================${NC}"
echo ""

# Verificar que Docker está corriendo
echo -e "${YELLOW}[1/4] Verificando que Docker está corriendo...${NC}"
if ! docker info > /dev/null 2>&1; then
    echo -e "${RED}❌ Error: Docker no está corriendo. Por favor inicia Docker.${NC}"
    exit 1
fi
echo -e "${GREEN}✅ Docker está corriendo${NC}"
echo ""

# Verificar que los contenedores están levantados
echo -e "${YELLOW}[2/4] Verificando que los contenedores están corriendo...${NC}"
if ! docker ps --filter "name=catalog-api" --filter "status=running" -q | grep -q .; then
    echo -e "${RED}❌ Error: El contenedor 'catalog-api' no está corriendo.${NC}"
    echo -e "${YELLOW}   Ejecuta: docker-compose up -d${NC}"
    exit 1
fi

if ! docker ps --filter "name=catalogdb" --filter "status=running" -q | grep -q .; then
    echo -e "${RED}❌ Error: El contenedor 'catalogdb' no está corriendo.${NC}"
    echo -e "${YELLOW}   Ejecuta: docker-compose up -d${NC}"
    exit 1
fi
echo -e "${GREEN}✅ Contenedores están corriendo${NC}"
echo ""

# Esperar a que PostgreSQL esté completamente listo
echo -e "${YELLOW}[3/4] Esperando a que PostgreSQL esté listo...${NC}"
sleep 5
echo -e "${GREEN}✅ PostgreSQL listo${NC}"
echo ""

# Aplicar migraciones
echo -e "${YELLOW}[4/4] Aplicando migraciones de Catalog Service...${NC}"

# Detectar si estamos en WSL y encontrar la ruta correcta del proyecto
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Si estamos en WSL, buscar la ruta en /mnt/c/ o donde esté el proyecto
if grep -qi microsoft /proc/version; then
    echo -e "${CYAN}ℹ️  Detectado WSL (Windows Subsystem for Linux)${NC}"
    
    # Buscar el proyecto en las ubicaciones comunes de WSL
    PROJECT_PATH=""
    
    # Intentar encontrar el proyecto
    if [ -d "$SCRIPT_DIR/../src" ]; then
        PROJECT_PATH="$SCRIPT_DIR/../src/Services/Catalog/Catalog.API/Catalog.API"
    elif [ -d "/mnt/c/Users/Andres/Desktop/DEVELOP/PROYECTOS CREADOS CON IA/EcommerceMS/src" ]; then
        PROJECT_PATH="/mnt/c/Users/Andres/Desktop/DEVELOP/PROYECTOS CREADOS CON IA/EcommerceMS/src/Services/Catalog/Catalog.API/Catalog.API"
    else
        # Buscar recursivamente
        echo -e "${YELLOW}Buscando la ruta del proyecto...${NC}"
        SEARCH_BASE="/mnt/c/Users/Andres"
        PROJECT_PATH=$(find "$SEARCH_BASE" -type d -name "EcommerceMS" 2>/dev/null | head -1)
        if [ -n "$PROJECT_PATH" ]; then
            PROJECT_PATH="$PROJECT_PATH/src/Services/Catalog/Catalog.API/Catalog.API"
        fi
    fi
    
    if [ ! -d "$PROJECT_PATH" ]; then
        echo -e "${RED}❌ Error: No se pudo encontrar el proyecto.${NC}"
        echo -e "${YELLOW}Por favor, ejecuta el script desde la carpeta docker-compose del proyecto.${NC}"
        exit 1
    fi
else
    # Linux/Mac normal
    PROJECT_PATH="$SCRIPT_DIR/../src/Services/Catalog/Catalog.API/Catalog.API"
fi

echo -e "${CYAN}Ruta del proyecto: $PROJECT_PATH${NC}"

# Ir a la carpeta del proyecto
cd "$PROJECT_PATH" || {
    echo -e "${RED}❌ Error: No se pudo acceder a la carpeta del proyecto${NC}"
    exit 1
}

# Aplicar migraciones
if dotnet ef database update \
    --project ../../Catalog.Infrastructure/Catalog.Infrastructure.csproj \
    --startup-project Catalog.API.csproj \
    --verbose; then
    
    echo ""
    echo -e "${GREEN}✅ Migraciones aplicadas exitosamente!${NC}"
    echo ""
    echo -e "${CYAN}Puedes probar los servicios en:${NC}"
    echo -e "  - API Gateway:    http://localhost:5000/health"
    echo -e "  - Catalog API:    http://localhost:5001/swagger"
    echo -e "  - Basket API:     http://localhost:5002/swagger"
    echo ""
else
    echo ""
    echo -e "${RED}❌ Error al aplicar migraciones${NC}"
    exit 1
fi

echo -e "${CYAN}========================================"
echo -e "           ¡Proceso Completado!         "
echo -e "========================================${NC}"