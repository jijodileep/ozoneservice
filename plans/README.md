# Ozone Mobile Service — Execution Plans

Small, focused plans to execute **one chunk at a time**. Complete each plan fully before starting the next.

**Master architecture:** see the main plan **Mobile Service SaaS** in the Cursor plans panel (full domain model, API layout, product rules).

## How to use

1. Open the next pending chunk plan below.
2. Say **"execute chunk-01"** (or the chunk number) in Cursor to implement it.
3. Verify **Done criteria** in that plan.
4. Mark status `done` in this index, then move to the next chunk.
5. **Git at chunk start** (not at the end). When you begin a chunk, run:

```powershell
.\scripts\start-chunk.ps1 -Chunk "05" -Title "Authorization"
```

This commits and pushes any completed work from the previous chunk, then marks the new chunk as current. Implementation for the new chunk stays uncommitted until you start the *next* chunk (or commit manually).

## Milestones

| Milestone | Chunks | Outcome |
|-----------|--------|---------|
| MVP | 01–17 | Jobs on mobile, view on web |
| Full product | 01–30 | Billing, inventory, reports |
| Launch | 01–32 | Production deploy |
| SaaS platform | 01–35 | Super admin, plans, device login control |

## Chunk index

| # | Plan | Wave | Depends on | Status |
|---|------|------|------------|--------|
| 01 | [chunk-01-foundation.md](chunk-01-foundation.md) | Foundation | — | done |
| 02 | [chunk-02-ef-base.md](chunk-02-ef-base.md) | Foundation | 01 | done |
| 03 | [chunk-03-auth-api.md](chunk-03-auth-api.md) | Foundation | 02 | done |
| 04 | [chunk-04-multi-tenancy.md](chunk-04-multi-tenancy.md) | Foundation | 03 | done |
| 05 | [chunk-05-authorization.md](chunk-05-authorization.md) | Foundation | 04 | **next** |
| 06 | [chunk-06-angular-shell.md](chunk-06-angular-shell.md) | Client shells | 05 | pending |
| 07 | [chunk-07-flutter-shell.md](chunk-07-flutter-shell.md) | Client shells | 05 | pending |
| 08 | [chunk-08-branches.md](chunk-08-branches.md) | Setup | 06 | pending |
| 09 | [chunk-09-users.md](chunk-09-users.md) | Setup | 08 | pending |
| 10 | [chunk-10-mobile-masters.md](chunk-10-mobile-masters.md) | Setup | 08 | pending |
| 11 | [chunk-11-customer-api.md](chunk-11-customer-api.md) | Customers | 07, 10 | pending |
| 12 | [chunk-12-flutter-customers.md](chunk-12-flutter-customers.md) | Customers | 11 | pending |
| 13 | [chunk-13-job-api-basics.md](chunk-13-job-api-basics.md) | Jobs | 12 | pending |
| 14 | [chunk-14-flutter-jobs-list.md](chunk-14-flutter-jobs-list.md) | Jobs | 13 | pending |
| 15 | [chunk-15-job-status-api.md](chunk-15-job-status-api.md) | Jobs | 14 | pending |
| 16 | [chunk-16-flutter-job-detail.md](chunk-16-flutter-job-detail.md) | Jobs | 15 | pending |
| 17 | [chunk-17-angular-job-view.md](chunk-17-angular-job-view.md) | Jobs | 16 | pending |
| 18 | [chunk-18-job-photos.md](chunk-18-job-photos.md) | Jobs | 16 | pending |
| 19 | [chunk-19-parts-master.md](chunk-19-parts-master.md) | Inventory | 08 | pending |
| 20 | [chunk-20-stock-api.md](chunk-20-stock-api.md) | Inventory | 19 | pending |
| 21 | [chunk-21-flutter-stock.md](chunk-21-flutter-stock.md) | Inventory | 20 | pending |
| 22 | [chunk-22-parts-on-job.md](chunk-22-parts-on-job.md) | Inventory | 20 | pending |
| 23 | [chunk-23-flutter-parts-on-job.md](chunk-23-flutter-parts-on-job.md) | Inventory | 22 | pending |
| 24 | [chunk-24-angular-stock-view.md](chunk-24-angular-stock-view.md) | Inventory | 20 | pending |
| 25 | [chunk-25-estimates.md](chunk-25-estimates.md) | Billing | 16 | pending |
| 26 | [chunk-26-invoices.md](chunk-26-invoices.md) | Billing | 25 | pending |
| 27 | [chunk-27-payments.md](chunk-27-payments.md) | Billing | 26 | pending |
| 28 | [chunk-28-customer-list.md](chunk-28-customer-list.md) | Billing | 27 | pending |
| 29 | [chunk-29-pdf-invoice.md](chunk-29-pdf-invoice.md) | Billing | 26 | pending |
| 30 | [chunk-30-reports.md](chunk-30-reports.md) | Launch | 28 | pending |
| 31 | [chunk-31-onboarding.md](chunk-31-onboarding.md) | Launch | 08 | pending |
| 32 | [chunk-32-deploy.md](chunk-32-deploy.md) | Launch | 30 | pending |
| 33 | [chunk-33-super-admin-shops.md](chunk-33-super-admin-shops.md) | SaaS | 08, 34 | done |
| 34 | [chunk-34-saas-plans-limits.md](chunk-34-saas-plans-limits.md) | SaaS | 04 | done |
| 35 | [chunk-35-device-session-control.md](chunk-35-device-session-control.md) | SaaS | 03, 34 | pending |

## Sprint grouping (optional)

| Sprint | Chunks |
|--------|--------|
| 1 | 01–07 |
| 2 | 08–12 |
| 3 | 13–18 |
| 4 | 19–24 |
| 5 | 25–29 |
| 6 | 30–32 |
| 7 | 33–35 |

## Key product rules (all chunks)

- **Stack:** .NET 10, Angular web, Flutter mobile, PostgreSQL
- **Entries:** all operational CRUD on **Flutter**; web is setup + view + reports
- **Customers:** shared by mobile number per tenant; list shows **invoiced only**
- **Onboarding:** **Super Admin creates shops** with default ShopAdmin (chunk 33)
- **Plans:** user/branch limits per subscription plan (chunk 34)
- **Sessions:** block multiple device logins per plan (chunk 35)
