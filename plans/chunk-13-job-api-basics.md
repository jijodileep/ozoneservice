# Chunk 13: Job API — Basics

**Wave:** 4 — Service jobs  
**Depends on:** [chunk-12](chunk-12-flutter-customers.md)  
**Status:** pending

## Goal

Service job create, list, and get — filtered by branch.

## Tasks

- [ ] `ServiceJob` entity: customer, device, branch, problem, status, technician, dates
- [ ] `POST /api/jobs`, `GET /api/jobs`, `GET /api/jobs/{id}`
- [ ] Filter: branch, date range, status
- [ ] Initial status: `Received`
- [ ] Job number auto-generate per branch (e.g. `BR01-0001`)

## Done criteria

- Create job linked to customer + device + branch
- List returns only current branch jobs

## Next chunk

[chunk-14-flutter-jobs-list.md](chunk-14-flutter-jobs-list.md)
