# Chunk 08: Branches

**Wave:** 2 — Setup & masters  
**Depends on:** [chunk-06](chunk-06-angular-shell.md)  
**Status:** done

## Goal

Branch CRUD API and Angular setup screens (web only).

## Tasks

- [x] `Branch` entity: `Code`, `Name`, `Address`, `Phone`, `GstNumber`, `IsActive`
- [x] CRUD API: `GET/POST/PUT/DELETE /api/branches`
- [x] FluentValidation for branch DTOs
- [x] Angular: branch list, create/edit form, deactivate
- [x] TenantAdmin sees all branches; ShopAdmin sees own branch

## Done criteria

- Create branch via Angular → appears in list
- Branch scoped to current tenant
- Swagger CRUD works

## Next chunk

[chunk-09-users.md](chunk-09-users.md) or [chunk-10-mobile-masters.md](chunk-10-mobile-masters.md)
