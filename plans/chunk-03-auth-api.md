# Chunk 03: Auth API

**Wave:** 0 — Foundation  
**Depends on:** [chunk-02](chunk-02-ef-base.md)  
**Status:** pending

## Goal

ASP.NET Core Identity + JWT authentication: login, refresh, and current user endpoint.

## Tasks

- [ ] Create `ApplicationUser` extending IdentityUser (add `TenantId`, `DisplayName`)
- [ ] Configure Identity + EF stores on `AppDbContext`
- [ ] JWT settings in `appsettings.json` (secret, issuer, expiry)
- [ ] Endpoints:
  - `POST /api/auth/login`
  - `POST /api/auth/refresh`
  - `GET /api/auth/me`
- [ ] Add Swagger + JWT bearer auth in Swagger UI
- [ ] Seed one dev user (optional, for local testing)

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

## Next chunk

[chunk-04-multi-tenancy.md](chunk-04-multi-tenancy.md)
