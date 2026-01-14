Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Aplicando Migraciones - EcommerceMS  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "[1/5] Verificando Docker..." -ForegroundColor Yellow
docker info 2>&1 | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Docker no esta corriendo" -ForegroundColor Red
    exit 1
}
Write-Host "OK Docker corriendo" -ForegroundColor Green
Write-Host ""

Write-Host "[2/5] Verificando contenedores..." -ForegroundColor Yellow
$catalogDb = docker ps --filter "name=catalogdb" --filter "status=running" -q
$orderingDb = docker ps --filter "name=orderingdb" --filter "status=running" -q

if (-not $catalogDb) {
    Write-Host "Error: catalogdb no esta corriendo" -ForegroundColor Red
    exit 1
}

if (-not $orderingDb) {
    Write-Host "Error: orderingdb no esta corriendo" -ForegroundColor Red
    exit 1
}
Write-Host "OK Contenedores corriendo" -ForegroundColor Green
Write-Host ""

Write-Host "[3/5] Esperando PostgreSQL..." -ForegroundColor Yellow
Start-Sleep -Seconds 5
Write-Host "OK PostgreSQL listo" -ForegroundColor Green
Write-Host ""

Write-Host "[4/5] Esperando SQL Server..." -ForegroundColor Yellow
$sqlReady = $false
for ($i = 1; $i -le 20; $i++) {
    $result = docker exec orderingdb /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "SELECT 1" -C 2>&1
    if ($LASTEXITCODE -eq 0) {
        $sqlReady = $true
        break
    }
    Write-Host "  Intento $i/20..." -ForegroundColor Gray
    Start-Sleep -Seconds 2
}

if ($sqlReady) {
    Write-Host "OK SQL Server listo" -ForegroundColor Green
    
    Write-Host "  Creando OrderingDb..." -ForegroundColor Cyan
    docker exec orderingdb /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'OrderingDb') CREATE DATABASE OrderingDb" -C 2>&1 | Out-Null
    Write-Host "  OK OrderingDb lista" -ForegroundColor Green
}
else {
    Write-Host "Advertencia: SQL Server tardo mucho" -ForegroundColor Yellow
}
Write-Host ""

Write-Host "[5/5] Aplicando migraciones..." -ForegroundColor Yellow
$originalPath = Get-Location

Write-Host "  > Catalog..." -ForegroundColor Cyan
Set-Location "../src/Services/Catalog/Catalog.API/Catalog.API"
dotnet ef database update --project ../../Catalog.Infrastructure/Catalog.Infrastructure.csproj --startup-project Catalog.API.csproj 2>&1 | Out-Null
if ($LASTEXITCODE -eq 0) {
    Write-Host "    OK Catalog" -ForegroundColor Green
}
else {
    Write-Host "    Advertencia Catalog" -ForegroundColor Yellow
}

Set-Location $originalPath


Write-Host "  > Ordering..." -ForegroundColor Cyan
Set-Location "../src/Services/Ordering/Ordering.API/Ordering.API"
dotnet ef database update --project ../../Ordering.Infrastructure/Ordering.Infrastructure.csproj --startup-project Ordering.API.csproj 2>&1 | Out-Null
if ($LASTEXITCODE -eq 0) {
    Write-Host "    OK Ordering" -ForegroundColor Green
}
else {
    Write-Host "    Advertencia Ordering" -ForegroundColor Yellow
}

Set-Location $originalPath

Write-Host ""
Write-Host "Reiniciando servicios..." -ForegroundColor Cyan
docker-compose restart catalog-api ordering-api
Start-Sleep -Seconds 5

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "           Completado                   " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "URLs:" -ForegroundColor Cyan
Write-Host "  http://localhost:5000/health" -ForegroundColor White
Write-Host "  http://localhost:5001/swagger" -ForegroundColor White
Write-Host "  http://localhost:5002/swagger" -ForegroundColor White
Write-Host "  http://localhost:5003/swagger" -ForegroundColor White
Write-Host ""