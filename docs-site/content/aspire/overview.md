# Aspire Overview

Template-net10 integrates with **.NET Aspire** for local orchestration, telemetry, health checks, and resilience.

## Projects

| Project | Purpose |
|---------|---------|
| `Template-net10.AppHost` | Aspire orchestrator — starts the API |
| `Template-net10.ServiceDefaults` | Shared telemetry, health, HTTP resilience |
| `src/API` | The web API project |

## Run via Aspire

```powershell
dotnet run --project Template-net10.AppHost
```

The AppHost registers the API:

```csharp
builder.AddProject<Projects.Template_net10_API>("template-net10-api");
```

## Service Defaults

`ServiceDefaults/Extensions.cs` provides:

- OpenTelemetry tracing and metrics
- Health check endpoints
- HTTP client resilience (retries, circuit breaker)
- Service discovery helpers

The API calls `builder.AddServiceDefaults()` in `Program.cs`.

## Dashboard

Aspire prints a dashboard URL to the console showing:

- Running resources and their endpoints
- Logs and traces
- Environment variables (non-secret)

## Direct API Run

For quick iteration without Aspire:

```powershell
dotnet run --project src/API
```

Aspire adds observability but is not required for the API to function.

## Related

- [Running the App](/docs/getting-started/running-the-app)
- [Architecture Overview](/docs/architecture/overview)
