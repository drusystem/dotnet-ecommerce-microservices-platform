# Script para levantar todo el sistema y aplicar migraciones
# Uso: .\start-all.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "    Iniciando EcommerceMS Completo     " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Paso 1: Levantar contenedores
Write-Host "[1/2] Levantando contenedores con Docker Compose..." -ForegroundColor Yellow
docker-compose up -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error al levantar contenedores" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Contenedores levantados" -ForegroundColor Green
Write-Host ""

# Paso 2: Esperar y aplicar migraciones
Write-Host "[2/2] Esperando 10 segundos antes de aplicar migraciones..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host ""
& .\apply-migrations.ps1