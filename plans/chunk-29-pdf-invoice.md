# Chunk 29: PDF Invoice

**Wave:** 6 — Billing  
**Depends on:** [chunk-26](chunk-26-invoices.md)  
**Status:** pending

## Goal

Generate PDF invoice; share from mobile.

## Tasks

- [ ] Add QuestPDF (or similar) to Infrastructure
- [ ] `GET /api/invoices/{id}/pdf` returns PDF stream
- [ ] Template: shop name, GST, line items, totals
- [ ] Flutter: share/open PDF link on delivered job
- [ ] Angular: download PDF button on invoice detail

## Done criteria

- PDF opens with correct invoice data
- Share sheet works on Android/iOS

## Next chunk

[chunk-30-reports.md](chunk-30-reports.md)
