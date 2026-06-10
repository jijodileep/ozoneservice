# Chunk 34: SaaS Subscription Plans & Limits

**Wave:** 8 — SaaS platform  
**Depends on:** [chunk-04](chunk-04-multi-tenancy.md)  
**Status:** done

## Goal

Subscription plans with **user and branch limits**; enforce on create operations.

## Tasks

- [x] `SubscriptionPlan` entity:
  - `Name`, `Code`, `MaxUsers`, `MaxBranches`, `MaxDevicesPerUser`
  - `AllowWebLogin`, `AllowMobileLogin`, `IsActive`
- [x] `Tenant.SubscriptionPlanId`, `SubscriptionExpiresAt` (optional)
- [x] CRUD API `/api/platform/plans` (super admin only; GET + POST)
- [x] `POST /api/platform/shops/{id}/assign-plan`
- [x] `ISubscriptionLimitService` — ready for chunk 08/09 user & branch create
- [x] Return clear error: `403 Plan limit reached (max N users)`
- [x] Enforce on assign-plan when usage exceeds new plan limits
- [x] Angular: plan list/create + shop usage vs limits
- [ ] **Wire enforce on create** in chunk 08 (`POST /api/branches`) and chunk 09 (`POST /api/users`)

## Example seed plans

| Code | MaxUsers | MaxBranches | MaxDevices/user |
|------|----------|-------------|-----------------|
| STARTER | 3 | 1 | 1 |
| PRO | 10 | 3 | 1 |
| ENTERPRISE | 50 | 20 | 2 |

## Done criteria

- Tenant on Starter cannot add 4th user
- Super admin can assign plan to tenant
- Limits visible on platform shop detail

## Next chunk

[chunk-33-super-admin-shops.md](chunk-33-super-admin-shops.md) (if not done) or [chunk-35](chunk-35-device-session-control.md)
