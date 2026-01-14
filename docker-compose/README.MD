## 🚀 Ejecutar el Sistema con Docker

### Inicio Rápido

**Windows:**
```powershell
cd docker-compose
.\start-all.ps1
```

**Linux/Mac:**
```bash
cd docker-compose
./start-all.sh
```

### Comandos Disponibles

#### Levantar Contenedores
```bash
cd docker-compose
docker-compose up -d
```

#### Aplicar Migraciones (después de levantar)
```bash
# Windows
.\apply-migrations.ps1

# Linux/Mac
./apply-migrations.sh
```

#### Ver Logs
```bash
docker-compose logs -f
```

#### Detener Todo
```bash
docker-compose down
```

#### Limpiar Todo (elimina datos)
```bash
# Windows
.\clean-all.ps1

# Linux/Mac
./clean-all.sh
```

### URLs de Acceso

- **API Gateway:** http://localhost:5000/health
- **Catalog API:** http://localhost:5001/swagger
- **Basket API:** http://localhost:5002/swagger