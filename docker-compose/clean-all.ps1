Write-Host "========================================" -ForegroundColor Red
Write-Host "      Limpiando Sistema Completo       " -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Red
Write-Host ""
Write-Host "⚠️  ADVERTENCIA: Esto eliminará todos los datos!" -ForegroundColor Yellow
Write-Host "  - Todos los contenedores" -ForegroundColor Yellow
Write-Host "  - Todas las imágenes construidas" -ForegroundColor Yellow
Write-Host "  - Todos los volúmenes (bases de datos)" -ForegroundColor Yellow
Write-Host "  - Cache de Docker" -ForegroundColor Yellow
Write-Host ""

$confirmation = Read-Host "¿Estás seguro? (escribe 'SI' para confirmar)"

if ($confirmation -ne "SI") {
    Write-Host "Operación cancelada." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "[1/4] Deteniendo contenedores..." -ForegroundColor Yellow
docker-compose down

Write-Host ""
Write-Host "[2/4] Eliminando volúmenes..." -ForegroundColor Yellow
docker-compose down -v

Write-Host ""
Write-Host "[3/4] Eliminando imágenes..." -ForegroundColor Yellow
docker-compose down -v --rmi all

Write-Host ""
Write-Host "[4/4] Limpiando cache de Docker..." -ForegroundColor Yellow
docker builder prune -a -f

Write-Host ""
Write-Host "✅ Sistema limpiado completamente" -ForegroundColor Green
Write-Host ""
Write-Host "Para volver a iniciar, ejecuta:" -ForegroundColor Cyan
Write-Host "  .\start-all.ps1" -ForegroundColor White
Write-Host ""
Write-Host "O para desarrollo local:" -ForegroundColor Cyan
Write-Host "  # Levantar solo bases de datos:" -ForegroundColor White
Write-Host "  docker-compose up -d catalogdb basketdb orderingdb" -ForegroundColor Gray
Write-Host ""