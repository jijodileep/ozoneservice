# Chunk 26: Invoices

**Wave:** 6 — Billing  
**Depends on:** [chunk-25](chunk-25-estimates.md)  
**Status:** pending

## Goal

Line-item invoices on mobile; finalized invoice triggers customer list eligibility.

## Tasks

- [ ] `Invoice`, `InvoiceLine` entities
- [ ] `InvoiceStatus`: Draft, Finalized, Cancelled
- [ ] `POST /api/jobs/{id}/invoice` — labor + parts lines, tax fields (GST stub)
- [ ] `POST /api/invoices/{id}/finalize`
- [ ] Flutter: build invoice on delivery, finalize
- [ ] Angular: invoice list + detail (read-only)

## Done criteria

- Finalized invoice linked to job + customer
- Invoice lines match parts/labor on job

## Next chunks

- [chunk-27-payments.md](chunk-27-payments.md)
- [chunk-29-pdf-invoice.md](chunk-29-pdf-invoice.md)
