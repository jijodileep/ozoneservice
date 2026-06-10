# Chunk 05: Authorization & Roles

**Wave:** 0 — Foundation  
**Depends on:** [chunk-04](chunk-04-multi-tenancy.md)  
**Status:** pending

## Goal

Role-based access, branch scoping, and enforce **mobile-only writes** for operational data.

## Tasks

- [ ] Define roles: `TenantAdmin`, `ShopAdmin`, `ShopStaff`, `Accountant`
- [ ] Seed roles; assign to dev users
- [ ] `IBranchContext` — selected branch from header claim or user default
- [ ] Authorization policies:
  - `SetupWrite` — TenantAdmin, ShopAdmin (web setup)
  - `OperationalWrite` — ShopStaff, ShopAdmin (mobile entries)
  - `ReportsRead` — TenantAdmin, ShopAdmin, Accountant
- [ ] Document policy matrix in `Shared/AuthorizationPolicies.cs`
- [ ] Sample protected endpoint to validate policies

## Files to create/modify

- `Shared/Roles.cs`, `AuthorizationPolicies.cs`
- `Application/Interfaces/IBranchContext.cs`
- `Infrastructure/Authorization/BranchContext.cs`
- `Api/Program.cs` (policy registration)

## Done criteria

- Roles assigned and enforced on test endpoints
- Operational POST rejected for Accountant role
- Setup POST allowed for ShopAdmin on web policy

## Verify

- ShopStaff token → can hit operational write test endpoint
- Accountant token → 403 on operational write

## Next chunks (can parallelize)

- [chunk-06-angular-shell.md](chunk-06-angular-shell.md)
- [chunk-07-flutter-shell.md](chunk-07-flutter-shell.md)
