# Chunk 27: Payments

**Wave:** 6 — Billing  
**Depends on:** [chunk-26](chunk-26-invoices.md)  
**Status:** pending

## Goal

Record payments on mobile; support partial payments.

## Tasks

- [ ] `Payment` entity: amount, method (Cash, UPI, Card), reference, date
- [ ] `POST /api/invoices/{id}/payments`
- [ ] Invoice balance = total − sum(payments)
- [ ] Flutter: payment screen with method picker
- [ ] Angular: payment history on invoice detail

## Done criteria

- Partial payment updates balance
- Full payment marks invoice paid

## Next chunk

[chunk-28-customer-list.md](chunk-28-customer-list.md)
