# Ozone Mobile Service



Mobile Service Center management SaaS — multi-branch shops, .NET 10 API, Angular web admin, Flutter mobile entries.



## Prerequisites



- [.NET 10 SDK](https://dotnet.microsoft.com/download)

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Windows)



## Quick start



### 1. Configure environment



```powershell

copy .env.example .env

```



### 2. Start PostgreSQL



```powershell

docker compose up -d

```



### 3. Build and run the API



```powershell

dotnet build src/OzoneMobileService.slnx

dotnet run --project src/OzoneMobileService.Api --launch-profile http

```



### 4. Verify



```powershell

curl http://localhost:5055/health

```



Expected: `{"status":"healthy","database":"connected","timestamp":"..."}`

### 5. Angular web admin (dev)

```powershell
cd web/ozone-admin
npm start
```

Open http://localhost:4200 and sign in with a tenant web user (see [`docs/dev-credentials.md`](docs/dev-credentials.md)).

### 6. API docs & auth (dev)

- Swagger UI: http://localhost:5055/swagger
- Dev login: `POST /api/auth/login`
- **All dev usernames & passwords:** [`docs/dev-credentials.md`](docs/dev-credentials.md)

Platform API (Bearer token required):

- `GET /api/platform/plans`
- `POST /api/platform/shops` — creates shop + default branch + ShopAdmin
- `GET /api/platform/shops`
- `PATCH /api/platform/shops/{id}/suspend`

At the start of each chunk: `.\scripts\start-chunk.ps1 -Chunk "NN" -Title "..."`

## Solution structure



```

src/

├── OzoneMobileService.slnx

├── OzoneMobileService.Api/           # ASP.NET Core Web API

├── OzoneMobileService.Application/   # Use cases, DTOs

├── OzoneMobileService.Domain/        # Entities, domain rules

├── OzoneMobileService.Infrastructure/ # EF Core, external services

└── OzoneMobileService.Shared/        # Shared constants

```



## PostgreSQL (Docker)



```powershell

docker compose ps          # postgres should be healthy

docker compose down        # stop (keeps data)

docker compose down -v     # stop and delete data

```



Connect with psql:



```powershell

docker exec -it ozone-postgres psql -U ozone -d ozone_mobile_service

```



## Connection string



Configured in `src/OzoneMobileService.Api/appsettings.Development.json`:



```

Host=localhost;Port=5432;Database=ozone_mobile_service;Username=ozone;Password=ozone_dev_password

```



## Default database credentials

See [`docs/dev-credentials.md`](docs/dev-credentials.md#postgresql-docker).



## Execution plans



Implementation is split into small chunks under [`plans/`](plans/README.md). Chunks 01–07 and 33–34 are complete; next is [chunk-08](plans/chunk-08-branches.md).

### Flutter shop app (`mobile/ozone_shop`)

```powershell
cd mobile/ozone_shop
flutter pub get
flutter run
# Physical device on LAN:
flutter run --dart-define=API_BASE_URL=http://<your-pc-ip>:5055
```

Default API URL: `http://localhost:5055` (Windows/macOS/iOS simulator), `http://10.0.2.2:5055` (Android emulator). Dev login: `staff@localhost.dev` / `Staff@123`.

### EF Core migrations

```powershell
dotnet ef migrations add <Name> --project src/OzoneMobileService.Infrastructure --startup-project src/OzoneMobileService.Api
dotnet ef database update --project src/OzoneMobileService.Infrastructure --startup-project src/OzoneMobileService.Api
```


