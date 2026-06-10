# Chunk 15: Job Status API

**Wave:** 4 — Service jobs  
**Depends on:** [chunk-14](chunk-14-flutter-jobs-list.md)  
**Status:** pending

## Goal

Full status workflow with audit history and diagnosis fields.

## Tasks

- [ ] `JobStatus` enum: Received, Diagnosing, WaitingApproval, WaitingParts, InRepair, QualityCheck, Ready, Delivered
- [ ] `JobStatusHistory` entity (timestamp, user, from/to status)
- [ ] `JobDiagnosis` fields on job or separate entity
- [ ] `PATCH /api/jobs/{id}/status` with validation rules
- [ ] `POST /api/jobs/{id}/assign-technician`

## Done criteria

- Invalid transitions rejected (e.g. Delivered → Received)
- History timeline returned on job detail

## Next chunk

[chunk-16-flutter-job-detail.md](chunk-16-flutter-job-detail.md)
