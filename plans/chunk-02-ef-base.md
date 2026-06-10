# Chunk 02: EF Core Base

**Wave:** 0 — Foundation  
**Depends on:** [chunk-01](chunk-01-foundation.md)  
**Status:** done

## Goal

Add EF Core 10 + PostgreSQL, base entity pattern, and first migration.

## Tasks

- [x] Add NuGet: `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`, `Design` (Api)
- [x] Create `BaseEntity` in Domain: `Id`, `TenantId`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`
- [x] Create `ITenantEntity` interface for tenant-scoped entities
- [x] Create `AppDbContext` in Infrastructure
- [x] Register DbContext in Api `Program.cs` with connection string
- [x] Add `dotnet ef` tooling; create initial empty migration
- [x] Apply migration: `dotnet ef database update`

## Files to create/modify

- `Domain/Common/BaseEntity.cs`
- `Domain/Common/ITenantEntity.cs`
- `Infrastructure/Persistence/AppDbContext.cs`
- `Infrastructure/DependencyInjection.cs`
- `Api/Program.cs`

## Done criteria

- Migration applies to PostgreSQL without errors
- `AppDbContext` connects on API startup
- Tables created in `ozone_mobile_service` database

## Verify

```powershell
dotnet ef migrations add Initial --project src/OzoneMobileService.Infrastructure --startup-project src/OzoneMobileService.Api
dotnet ef database update --project src/OzoneMobileService.Infrastructure --startup-project src/OzoneMobileService.Api
```

## Next chunk

[chunk-03-auth-api.md](chunk-03-auth-api.md)
