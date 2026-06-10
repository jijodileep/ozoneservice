# Chunk 04: Multi-Tenancy

**Wave:** 0 — Foundation  
**Depends on:** [chunk-03](chunk-03-auth-api.md)  
**Status:** pending

## Goal

Tenant entity, tenant resolution from JWT, and EF global query filter on `TenantId`.

## Tasks

- [ ] Create `Tenant` entity: `Id`, `Name`, `Code`, `IsActive`, `SubscriptionPlanId`, `SubscriptionExpiresAt`
- [ ] Add `TenantId` to `ApplicationUser`; migration
- [ ] `ITenantContext` service (current tenant from JWT claim)
- [ ] `TenantResolutionMiddleware` or claims transformation
- [ ] EF global query filter: `entity.TenantId == _tenantContext.TenantId`
- [ ] Include `tenant_id` claim in JWT on login
- [ ] Seed one dev tenant linked to dev user

## Files to create/modify

- `Domain/Entities/Tenant.cs`
- `Application/Interfaces/ITenantContext.cs`
- `Infrastructure/MultiTenancy/TenantContext.cs`
- `Infrastructure/Persistence/AppDbContext.cs` (filters)
- `Api/Middleware/TenantMiddleware.cs`

## Done criteria

- All tenant-scoped queries auto-filter by JWT tenant
- Cross-tenant data access blocked
- Dev user belongs to dev tenant

## Verify

- Login → JWT contains `tenant_id`
- API queries only return current tenant data

## Next chunk

[chunk-05-authorization.md](chunk-05-authorization.md)
