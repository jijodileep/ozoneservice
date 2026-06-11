# Chunk 07: Flutter Shop Shell

**Wave:** 1 — Client shells  
**Depends on:** [chunk-05](chunk-05-authorization.md)  
**Status:** done

## Goal

Flutter shop app with login, secure token storage, API client, and branch context.

## Tasks

- [x] Create `mobile/ozone_shop` Flutter project
- [x] Add packages: `dio`, `flutter_secure_storage`, `provider` or `riverpod`
- [x] Login screen → `POST /api/auth/login`
- [x] Store tokens in secure storage
- [x] Dio client with auth + branch interceptors
- [x] After login, pin branch from user profile (no branch picker for ShopStaff)
- [x] Home placeholder screen with logout
- [x] API base URL config (dev/prod)

## Files to create

```
mobile/ozone_shop/
├── lib/core/api/
├── lib/core/auth/
├── lib/features/login/
└── lib/features/home/
```

## Done criteria

- Login persists across app restart
- Authenticated API call to `/api/auth/me` succeeds
- Branch ID sent on every request

## Verify

```powershell
cd mobile/ozone_shop
flutter run
# Login → home screen → restart app → still logged in
```

## Next chunk

[chunk-11-customer-api.md](chunk-11-customer-api.md) (after masters in chunk 10)
