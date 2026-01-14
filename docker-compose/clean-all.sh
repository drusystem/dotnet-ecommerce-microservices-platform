#!/bin/bash

RED='\033[0;31m'
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
CYAN='\033[0;36m'
NC='\033[0m'

echo -e "${RED}========================================"
echo -e "      Limpiando Sistema Completo       "
echo -e "========================================${NC}"
echo ""
echo -e "${YELLOW}⚠️  ADVERTENCIA: Esto eliminará todos los datos!${NC}"
echo ""

read -p "¿Estás seguro? (escribe 'SI' para confirmar): " confirmation

if [ "$confirmation" != "SI" ]; then
    echo -e "${YELLOW}Operación cancelada.${NC}"
    exit 0
fi

echo ""
echo -e "${YELLOW}Deteniendo contenedores...${NC}"
docker-compose down -v --rmi all

echo ""
echo -e "${YELLOW}Limpiando cache de Docker...${NC}"
docker builder prune -a -f

echo ""
echo -e "${GREEN}✅ Sistema limpiado completamente${NC}"
echo ""
echo -e "${CYAN}Para volver a iniciar, ejecuta:${NC}"
echo "  ./start-all.sh"