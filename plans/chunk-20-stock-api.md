# Chunk 20: Stock API

**Wave:** 5 — Inventory  
**Depends on:** [chunk-19](chunk-19-parts-master.md)  
**Status:** pending

## Goal

Branch stock levels and stock movement ledger.

## Tasks

- [ ] `BranchStock` (BranchId, PartId, Quantity)
- [ ] `StockMovement` (type: Receipt, Adjustment, Consumption, Transfer)
- [ ] `POST /api/inventory/receipt`, `POST /api/inventory/adjustment`
- [ ] `GET /api/inventory/stock?branchId=`
- [ ] `GET /api/inventory/movements`

## Done criteria

- Stock receipt increases branch quantity
- Movement log records every change

## Next chunks

- [chunk-21-flutter-stock.md](chunk-21-flutter-stock.md)
- [chunk-22-parts-on-job.md](chunk-22-parts-on-job.md)
- [chunk-24-angular-stock-view.md](chunk-24-angular-stock-view.md)
