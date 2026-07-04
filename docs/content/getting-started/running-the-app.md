# Running the App

## Build and Test

From the repository root:

```powershell
dotnet build Template-net10.slnx
dotnet test Tests/Template-net10.UnitTests/Template-net10.UnitTests.csproj
```

A task is not complete until `dotnet build` succeeds with zero errors and tests pass.

## Run via Aspire (Preferred)

Aspire orchestrates the API with telemetry, health checks, and resilience defaults:

```powershell
dotnet run --project Template-net10.AppHost
```

The AppHost project references `src/API` and starts it as a managed resource. Open the Aspire dashboard URL printed in the console to inspect endpoints and logs.

## Run the API Directly

For quick iteration without Aspire:

```powershell
dotnet run --project src/API
```

Swagger UI is available in Development at `/swagger`.

## Run the Documentation Site

This documentation site is a standalone React app:

```powershell
cd docs-site
npm install
npm run dev
```

Open the URL printed by Vite (typically `http://localhost:5173`).

## Locked DLLs While the API Is Running

If the API is running, a full-solution build may fail with `MSB3027` / `MSB3021` "file is locked" on `src/API/bin/...`. This is not a compile error. Either stop the running app, or build individual projects:

```powershell
dotnet build src/Application/Template-net10.Application.csproj
```

## Default Endpoints

| Endpoint | Description |
|----------|-------------|
| `/swagger` | OpenAPI / Swagger UI (Development) |
| `/hangfire` | Hangfire job dashboard |
| `/health` | Health check endpoint |
| `/api/v1/auth/login` | Authentication |

## Related

- [Aspire Overview](/docs/aspire/overview)
- [Testing Overview](/docs/testing/overview)
