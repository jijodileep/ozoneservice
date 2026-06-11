# Chunk 09: Users & Roles

**Wave:** 2 — Setup & masters  
**Depends on:** [chunk-08](chunk-08-branches.md)  
**Status:** done

## Goal

User management with branch assignment and role selection (web setup).

## Tasks

- [x] `UserBranch` join entity (UserId, BranchId)
- [x] API: `GET/POST/PUT /api/users`, assign branches, assign roles
- [x] **Enforce plan limit:** reject create if tenant active users >= `SubscriptionPlan.MaxUsers` (see [chunk-34](chunk-34-saas-plans-limits.md))
- [x] Angular: user list, create user, assign role + branch(es)
- [x] Password reset / initial password flow (simple)

## Done criteria

- Create ShopStaff user assigned to one branch
- That user can login on Flutter with correct branch context

## Next chunk

[chunk-10-mobile-masters.md](chunk-10-mobile-masters.md)
