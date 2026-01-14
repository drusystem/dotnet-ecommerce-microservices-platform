#EJECUTAR TODOS LOS SERVICIOS LOCALMENTE .\start-local.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Iniciando Sistema en Modo Desarrollo  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Paso 1: Levantar bases de datos
Write-Host "[1/4] Levantando bases de datos en Docker..." -ForegroundColor Yellow
Set-Location docker-compose
docker-compose up -d catalogdb basketdb orderingdb

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error al levantar bases de datos" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Bases de datos iniciadas" -ForegroundColor Green
Write-Host ""

# Paso 2: Esperar
Write-Host "[2/4] Esperando a que las bases de datos estén listas..." -ForegroundColor Yellow
Start-Sleep -Seconds 10
Write-Host "✅ Bases de datos listas" -ForegroundColor Green
Write-Host ""

# Paso 3: Compilar todo
Write-Host "[3/4] Compilando la solución..." -ForegroundColor Yellow
Set-Location ..
dotnet build --quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error al compilar" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Compilación exitosa" -ForegroundColor Green
Write-Host ""

# Paso 4: Aplicar migraciones
Write-Host "[4/4] Aplicando migraciones..." -ForegroundColor Yellow

# Catalog
Write-Host "  → Catalog Service..." -ForegroundColor Cyan
Set-Location src\Services\Catalog\Catalog.API\Catalog.API
$output = dotnet ef database update --project ..\..\Catalog.Infrastructure\Catalog.Infrastructure.csproj --startup-project Catalog.API.csproj 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️  Advertencia en Catalog migrations (puede que ya estén aplicadas)" -ForegroundColor Yellow
}

# Ordering
Write-Host "  → Ordering Service..." -ForegroundColor Cyan
Set-Location ..\..\..\..\Ordering\Ordering.API\Ordering.API
$output = dotnet ef database update --project ..\..\Ordering.Infrastructure\Ordering.Infrastructure.csproj --startup-project Ordering.API.csproj 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️  Advertencia en Ordering migrations (puede que ya estén aplicadas)" -ForegroundColor Yellow
}

Set-Location ..\..\..\..\..\..

Write-Host "✅ Migraciones aplicadas" -ForegroundColor Green
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "           Sistema Listo                " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Ahora ejecuta en terminales separadas:" -ForegroundColor Yellow
Write-Host ""
Write-Host "Terminal 1 (Catalog):" -ForegroundColor White
Write-Host "  cd src\Services\Catalog\Catalog.API\Catalog.API" -ForegroundColor Gray
Write-Host "  dotnet run" -ForegroundColor Gray
Write-Host ""
Write-Host "Terminal 2 (Basket):" -ForegroundColor White
Write-Host "  cd src\Services\Basket\Basket.API\Basket.API" -ForegroundColor Gray
Write-Host "  dotnet run" -ForegroundColor Gray
Write-Host ""
Write-Host "Terminal 3 (Ordering):" -ForegroundColor White
Write-Host "  cd src\Services\Ordering\Ordering.API\Ordering.API" -ForegroundColor Gray
Write-Host "  dotnet run" -ForegroundColor Gray
Write-Host ""
Write-Host "Terminal 4 (Gateway):" -ForegroundColor White
Write-Host "  cd src\ApiGateway\ApiGateway" -ForegroundColor Gray
Write-Host "  dotnet run" -ForegroundColor Gray
Write-Host ""
Write-Host "URLs de acceso:" -ForegroundColor Cyan
Write-Host "  Gateway:  http://localhost:5000/health" -ForegroundColor White
Write-Host "  Catalog:  http://localhost:5001/swagger" -ForegroundColor White
Write-Host "  Basket:   http://localhost:5002/swagger" -ForegroundColor White
Write-Host "  Ordering: http://localhost:5003/swagger" -ForegroundColor White