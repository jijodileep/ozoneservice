# Chunk 25: Estimates

**Wave:** 6 — Billing  
**Depends on:** [chunk-16](chunk-16-flutter-job-detail.md)  
**Status:** pending

## Goal

Pre-repair estimates on mobile; view on web.

## Tasks

- [ ] `Estimate` entity linked to job (labor + parts lines)
- [ ] `POST /api/jobs/{id}/estimate`
- [ ] `PATCH /api/jobs/{id}/estimate/approve` (customer approved flag)
- [ ] Flutter: create estimate from job, mark approved
- [ ] Angular: view estimate on job detail

## Done criteria

- Estimate created on mobile, visible on web

## Next chunk

[chunk-26-invoices.md](chunk-26-invoices.md)
