# Chunk 06: Angular Web Shell

**Wave:** 1 — Client shells  
**Depends on:** [chunk-05](chunk-05-authorization.md)  
**Status:** pending

## Goal

Angular admin app with login, JWT interceptor, layout, branch selector, and route guards.

## Tasks

- [ ] Create `web/ozone-admin` — Angular 19+ standalone, Angular Material
- [ ] Login page → calls `POST /api/auth/login`
- [ ] Store JWT in memory or sessionStorage
- [ ] HTTP interceptor: attach `Authorization` + `X-Branch-Id` headers
- [ ] App layout: sidebar, header, branch selector dropdown
- [ ] Route guards: `authGuard`, `roleGuard`
- [ ] Placeholder dashboard route after login
- [ ] Environment files: `apiUrl` for local API

## Files to create

```
web/ozone-admin/
├── src/app/core/auth/
├── src/app/core/interceptors/
├── src/app/layout/
├── src/app/features/login/
└── src/environments/
```

## Done criteria

- Login flow works against API
- Protected routes redirect to login when logged out
- Branch selector sends branch header on API calls

## Verify

```powershell
cd web/ozone-admin
npm start
# Login with dev user → see dashboard shell
```

## Next chunk

[chunk-08-branches.md](chunk-08-branches.md) (after 07 optional in parallel)
