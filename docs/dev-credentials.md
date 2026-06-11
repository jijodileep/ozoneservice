# Development credentials

**Development only.** Do not use these passwords in production.

Seeded automatically when the API runs in `Development` (`DevelopmentDataSeeder`). Source of truth: `src/OzoneMobileService.Shared/SeedConstants.cs`.

## Platform Super Admin

| Field | Value |
|-------|-------|
| Email | `superadmin@localhost.dev` |
| Password | `Super@123` |
| Role | `PlatformSuperAdmin` |

Uses `/api/platform/*` to create shops, plans, suspend tenants, etc. No tenant — platform-wide access only.

```powershell
$body = '{"email":"superadmin@localhost.dev","password":"Super@123"}'
Invoke-RestMethod -Uri http://localhost:5055/api/auth/login -Method Post -Body $body -ContentType "application/json"
```

## API login (`POST /api/auth/login`)

| Display name | Email | Password | Role | Tenant |
|--------------|-------|----------|------|--------|
| Platform Super Admin | `superadmin@localhost.dev` | `Super@123` | `PlatformSuperAdmin` | — (platform) |
| Dev Admin | `admin@localhost.dev` | `Admin@123` | `TenantAdmin` | Dev Service Center |
| Dev Shop Admin | `shopadmin@localhost.dev` | `Shop@12345` | `ShopAdmin` | Dev Service Center |
| Dev Shop Staff | `staff@localhost.dev` | `Staff@123` | `ShopStaff` | Dev Service Center |
| Dev Accountant | `accountant@localhost.dev` | `Account@123` | `Accountant` | Dev Service Center |

### Dev tenant & branch

| Item | Value |
|------|-------|
| Tenant name | Dev Service Center |
| Tenant code | `DEV` |
| Tenant ID | `00000000-0000-0000-0000-000000000001` |
| Default branch | Main Branch (`MAIN`) |
| Branch ID | `00000000-0000-0000-0000-000000000010` |

Shop users (ShopAdmin, ShopStaff, Accountant) are linked to the dev `MAIN` branch. Send `X-Branch-Id: 00000000-0000-0000-0000-000000000010` to select a branch explicitly.

## Shop admins created via Super Admin

When a super admin creates a shop (`POST /api/platform/shops`), the **ShopAdmin email and password come from the request body** — they are not fixed. Example from testing:

| Email | Password | Role | Notes |
|-------|----------|------|-------|
| `shopadmin@city01.dev` | `Shop@12345` | `ShopAdmin` | Created with shop provisioning API |

## PostgreSQL (Docker)

| Setting | Value |
|---------|-------|
| Host | `localhost` |
| Port | `5432` |
| Database | `ozone_mobile_service` |
| Username | `ozone` |
| Password | `ozone_dev_password` |

Connection string (see `appsettings.Development.json`):

```
Host=localhost;Port=5432;Database=ozone_mobile_service;Username=ozone;Password=ozone_dev_password
```

## Flutter shop app (`mobile/ozone_shop`)

| Field | Value |
|-------|-------|
| Default login | `staff@localhost.dev` / `Staff@123` |
| API (desktop/iOS sim) | `http://localhost:5055` |
| API (Android emulator) | `http://10.0.2.2:5055` |

```powershell
cd mobile/ozone_shop
flutter run
```

## Quick login — tenant admin (PowerShell)

```powershell
$body = '{"email":"admin@localhost.dev","password":"Admin@123"}'
Invoke-RestMethod -Uri http://localhost:5055/api/auth/login -Method Post -Body $body -ContentType "application/json"
```

Swagger: http://localhost:5055/swagger
