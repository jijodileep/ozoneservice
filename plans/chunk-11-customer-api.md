# Chunk 11: Customer API

**Wave:** 3 — Customers  
**Depends on:** [chunk-07](chunk-07-flutter-shell.md), [chunk-10](chunk-10-mobile-masters.md)  
**Status:** done

## Goal

Shared customer by mobile number; lookup and create endpoints.

## Tasks

- [x] `Customer` entity + `CustomerDevice`
- [x] `UNIQUE (TenantId, MobileNumber)` constraint
- [x] `PhoneNormalizer` utility (+91 / spaces)
- [x] `GET /api/customers/lookup?mobile=...`
- [x] `POST /api/customers` (mobile roles only)
- [x] `GET /api/customers/{id}` with devices + history stub

## Business rules

- Same phone = same customer across all branches
- Customer list endpoint deferred to chunk 28 (invoiced only)

## Done criteria

- Lookup finds existing customer by normalized phone
- Duplicate phone in same tenant returns 409

## Next chunk

[chunk-12-flutter-customers.md](chunk-12-flutter-customers.md)
