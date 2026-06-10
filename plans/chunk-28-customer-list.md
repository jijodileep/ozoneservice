# Chunk 28: Invoiced Customer List

**Wave:** 6 — Billing  
**Depends on:** [chunk-27](chunk-27-payments.md)  
**Status:** pending

## Goal

Customer list shows **invoiced customers only**; phone lookup remains separate.

## Tasks

- [ ] `GET /api/customers` — `WHERE EXISTS (finalized invoice)`
- [ ] Search by name/phone on invoiced set
- [ ] `GET /api/customers/{id}` — cross-branch job + invoice history
- [ ] Angular: customer list + detail (view only)
- [ ] Flutter: invoiced customer search (optional screen)

## Business rules

- Job-only customers NOT in list
- Lookup endpoint unchanged for new job intake

## Done criteria

- Customer with invoice appears in list
- Customer with job but no invoice does not

## Next chunk

[chunk-30-reports.md](chunk-30-reports.md)
