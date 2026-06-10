# Chunk 09: Users & Roles

**Wave:** 2 — Setup & masters  
**Depends on:** [chunk-08](chunk-08-branches.md)  
**Status:** pending

## Goal

User management with branch assignment and role selection (web setup).

## Tasks

- [ ] `UserBranch` join entity (UserId, BranchId)
- [ ] API: `GET/POST/PUT /api/users`, assign branches, assign roles
- [ ] Angular: user list, create user, assign role + branch(es)
- [ ] Password reset / initial password flow (simple)

## Done criteria

- Create ShopStaff user assigned to one branch
- That user can login on Flutter with correct branch context

## Next chunk

[chunk-10-mobile-masters.md](chunk-10-mobile-masters.md)
