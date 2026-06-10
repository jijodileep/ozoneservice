# Chunk 03: Auth API

**Wave:** 0 — Foundation  
**Depends on:** [chunk-02](chunk-02-ef-base.md)  
**Status:** done (pushed)

## Goal

ASP.NET Core Identity + JWT authentication: login, refresh, and current user endpoint.

## Tasks

- [x] Create `ApplicationUser` extending IdentityUser (add `TenantId`, `DisplayName`)
- [x] Configure Identity + EF stores on `AppDbContext`
- [x] JWT settings in `appsettings.json` (secret, issuer, expiry)
- [x] Endpoints:
  - `POST /api/auth/login`
  - `POST /api/auth/refresh`
  - `GET /api/auth/me`
- [x] Add Swagger + JWT bearer auth in Swagger UI
- [x] Seed one dev user (optional, for local testing)

## Files to create/modify

- `Domain/Entities/ApplicationUser.cs`
- `Application/DTOs/Auth/LoginRequest.cs`, `TokenResponse.cs`
- `Application/Services/IAuthService.cs`, `AuthService.cs`
- `Api/Controllers/AuthController.cs`
- `Infrastructure/Identity/IdentityConfiguration.cs`

## Done criteria

- Login returns access + refresh tokens
- `/api/auth/me` works with Bearer token
- Swagger documents auth endpoints

## Verify

- POST login with seed user → 200 + JWT
- GET `/api/auth/me` with token → user profile

## Deferred to later chunks

- **Device session / block multi-login:** [chunk-35](chunk-35-device-session-control.md)
- **Plan-based login limits:** [chunk-34](chunk-34-saas-plans-limits.md)

## Next chunk

[chunk-04-multi-tenancy.md](chunk-04-multi-tenancy.md)
