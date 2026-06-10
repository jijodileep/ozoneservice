# Chunk 35: Device Session Control (Block Multi-Login)

**Wave:** 8 — SaaS platform  
**Depends on:** [chunk-03](chunk-03-auth-api.md), [chunk-34](chunk-34-saas-plans-limits.md)  
**Status:** pending

## Goal

Block **multiple simultaneous logins** per user based on plan `MaxDevicesPerUser` (default: **1 device**).

## Tasks

- [ ] `UserSession` entity: `UserId`, `DeviceId`, `ClientType` (Web/Mobile), `RefreshTokenId`, `IpAddress`, `CreatedAt`, `LastActiveAt`, `RevokedAt`
- [ ] Login request includes `deviceId` + `clientType` (Flutter + Angular send on login)
- [ ] On login:
  - Count active sessions for user
  - If `active >= MaxDevicesPerUser` from tenant plan → **409 Conflict** `"Already logged in on another device"`
  - Else create session + issue tokens
- [ ] Refresh token linked to `UserSession`; revoke session on logout
- [ ] `POST /api/auth/logout` — revoke current session
- [ ] `POST /api/auth/logout-all` — ShopAdmin/TenantAdmin for a user (optional)
- [ ] `GET /api/auth/sessions` — list my active sessions (optional)
- [ ] Middleware: reject API calls if session revoked (validate session id in JWT claim)

## Business rules

- **Default:** 1 device per user (mobile OR web, not both if MaxDevicesPerUser=1).
- Enterprise plan may allow 2 devices (e.g. web + mobile).
- Super admin not subject to tenant plan device limits.

## Done criteria

- User logged in on phone → second login on another phone blocked
- Logout on device A → login on device B succeeds
- Plan with MaxDevicesPerUser=2 allows two active sessions

## Next chunk

Production hardening or chunk-32 deploy if not done
