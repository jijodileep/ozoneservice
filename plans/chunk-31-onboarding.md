# Chunk 31: Tenant Onboarding

**Wave:** 7 — Launch  
**Depends on:** [chunk-08](chunk-08-branches.md)  
**Status:** pending

## Goal

Self-service tenant registration: company + first admin user.

## Tasks

- [ ] `POST /api/tenants/register` — tenant name, admin email, password
- [ ] Create tenant + TenantAdmin user + default branch (optional)
- [ ] Angular public signup page (no auth required)
- [ ] Email verification stub (optional for MVP)
- [ ] `Tenant.IsActive` flag for approval workflow (optional)

## Done criteria

- New tenant can register and login as TenantAdmin
- Data isolated from other tenants

## Next chunk

[chunk-32-deploy.md](chunk-32-deploy.md)
