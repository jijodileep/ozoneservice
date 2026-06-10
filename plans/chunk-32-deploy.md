# Chunk 32: Deploy & CI/CD

**Wave:** 7 — Launch  
**Depends on:** [chunk-30](chunk-30-reports.md)  
**Status:** pending

## Goal

Production-ready pipeline and deployment.

## Tasks

- [ ] GitHub Actions: build .NET, run tests, build Angular
- [ ] Environment config: staging + production secrets
- [ ] Deploy API (Azure App Service or container)
- [ ] Deploy Angular static site
- [ ] `GET /health` + DB health check
- [ ] Serilog structured logging
- [ ] README: deployment section

## Done criteria

- CI passes on push to main
- Staging URL serves API + web
- Flutter points to staging API for QA build

## Next chunk

— Project MVP/launch complete. Optional: offline mobile, SMS notifications, subscription billing.
