Write-Host "Deteniendo servicios..." -ForegroundColor Yellow

cd docker-compose
docker-compose stop catalogdb basketdb orderingdb

Write-Host "✅ Servicios detenidos" -ForegroundColor Green
Write-Host ""
Write-Host "Presiona Ctrl+C en cada terminal de los servicios .NET" -ForegroundColor Cyan