#!/bin/bash

# Colores
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${CYAN}========================================"
echo -e "    Iniciando EcommerceMS Completo     "
echo -e "========================================${NC}"
echo ""

# Paso 1: Levantar contenedores
echo -e "${YELLOW}[1/2] Levantando contenedores con Docker Compose...${NC}"
docker-compose up -d

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Error al levantar contenedores${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Contenedores levantados${NC}"
echo ""

# Paso 2: Esperar y aplicar migraciones
echo -e "${YELLOW}[2/2] Esperando 10 segundos antes de aplicar migraciones...${NC}"
sleep 10

echo ""
./apply-migrations.sh