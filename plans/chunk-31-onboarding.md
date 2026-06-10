# Chunk 31: Optional Self-Registration

**Wave:** 7 — Launch  
**Depends on:** [chunk-08](chunk-08-branches.md), [chunk-34](chunk-34-saas-plans-limits.md)  
**Status:** pending

## Goal

Optional **public signup** — **not the primary flow**. Primary onboarding is **[chunk-33](chunk-33-super-admin-shops.md)** (Super Admin creates shops).

## Tasks

- [ ] `POST /api/tenants/register` — only if `PlatformSettings.AllowSelfRegistration = true`
- [ ] Create tenant + TenantAdmin + default branch + assign default Starter plan
- [ ] Angular public signup page (hidden/disabled by default)
- [ ] Email verification stub (optional)

## Done criteria

- Self-register disabled by default
- When enabled, new tenant gets Starter plan limits
- Super admin flow (chunk 33) remains primary

## Next chunk

[chunk-32-deploy.md](chunk-32-deploy.md)
