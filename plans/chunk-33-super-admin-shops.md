# Chunk 33: Super Admin — Create Shops

**Wave:** 8 — SaaS platform  
**Depends on:** [chunk-34](chunk-34-saas-plans-limits.md) (plans must exist first), [chunk-08](chunk-08-branches.md)  
**Status:** done

## Goal

**Platform Super Admin** creates a new shop (tenant) with a **default branch** and **default ShopAdmin** user.

## Tasks

- [x] Add role `PlatformSuperAdmin` (`TenantId` null on user)
- [x] Seed one super admin dev user
- [x] `POST /api/platform/shops` — body: shop name, code, plan id, default branch name, shop admin email, temp password
- [x] Transaction: create `Tenant` → `Branch` → `ApplicationUser` (ShopAdmin) → `UserBranch`
- [x] `GET /api/platform/shops` — list all tenants (super admin only)
- [x] `PATCH /api/platform/shops/{id}/suspend` — block tenant logins
- [ ] Angular **platform console**: shop list, create shop form (deferred — API ready)

## Business rules

- Primary onboarding path: **super admin creates shop**, not public signup.
- Default ShopAdmin can log into **web + mobile**.
- Shop Admin manages their shop staff within plan user limits (chunk 34).

## Done criteria

- Super admin creates shop → ShopAdmin can login
- ShopAdmin sees only their tenant data
- Suspended shop → login returns 403

## Next chunk

[chunk-35-device-session-control.md](chunk-35-device-session-control.md) (can run parallel with 34 if 34 done)
