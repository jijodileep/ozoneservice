# Chunk 01: Foundation — Repo & .NET Solution

**Wave:** 0 — Foundation  
**Depends on:** —  
**Status:** done

## Goal

Scaffold the monorepo skeleton: .NET 10 Clean Architecture solution, solution file, and confirm PostgreSQL via Docker.

## Already done

- [x] `docker-compose.yml` (PostgreSQL)
- [x] `.env.example`, `.gitignore`
- [x] `README.md` with `docker compose up` instructions

## Tasks

- [x] Initialize git repo (if not done)
- [x] Create solution `OzoneMobileService.slnx` under `src/`
- [x] Add 5 projects targeting `net10.0`:
  - `OzoneMobileService.Domain`
  - `OzoneMobileService.Application`
  - `OzoneMobileService.Infrastructure`
  - `OzoneMobileService.Api`
  - `OzoneMobileService.Shared`
- [x] Wire project references (Domain ← Application ← Infrastructure ← Api)
- [x] Minimal `Program.cs` — health endpoint `GET /health`
- [x] `appsettings.Development.json` with PostgreSQL connection string
- [x] Update root `README.md` with solution build/run commands

## Files to create

```
src/
├── OzoneMobileService.sln
├── OzoneMobileService.Domain/
├── OzoneMobileService.Application/
├── OzoneMobileService.Infrastructure/
├── OzoneMobileService.Api/
└── OzoneMobileService.Shared/
```

## Done criteria

- `dotnet build` succeeds on all projects
- `docker compose up -d` → postgres healthy
- `dotnet run --project src/OzoneMobileService.Api` → `GET /health` returns 200

## Verify

```powershell
docker compose ps
dotnet build src/OzoneMobileService.sln
dotnet run --project src/OzoneMobileService.Api
```

## Next chunk

[chunk-02-ef-base.md](chunk-02-ef-base.md)
