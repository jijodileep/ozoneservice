# Chunk 22: Parts on Job API

**Wave:** 5 — Inventory  
**Depends on:** [chunk-20](chunk-20-stock-api.md)  
**Status:** pending

## Goal

Record parts used on a job; auto-deduct branch stock.

## Tasks

- [ ] `JobPartUsage` (JobId, PartId, Quantity, UnitPrice)
- [ ] `POST /api/jobs/{id}/parts` — deducts `BranchStock`
- [ ] Reject if insufficient stock
- [ ] Include parts on job detail response

## Done criteria

- Adding part to job reduces stock
- Stock movement logged as Consumption

## Next chunk

[chunk-23-flutter-parts-on-job.md](chunk-23-flutter-parts-on-job.md)
