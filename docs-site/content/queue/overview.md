# Queue & Jobs Overview

Background processing uses **Hangfire** with SQL Server storage. Jobs support fire-and-forget, delayed, and recurring schedules.

## Components

| Component | Path |
|-----------|------|
| `IJobScheduler` | `Application/Abstractions/Jobs/IJobScheduler.cs` |
| `HangfireJobScheduler` | `Infrastructure/Jobs/HangfireJobScheduler.cs` |
| `IEmailJob` | `Application/Abstractions/Jobs/IEmailJob.cs` |
| `IMaintenanceJob` | `Application/Abstractions/Jobs/IMaintenanceJob.cs` |

## Dashboard

When `Queue:DashboardEnabled` is `true`, visit:

```
https://localhost:5001/hangfire
```

Requires an authenticated admin user.

## Recurring Jobs

Registered at startup in `HangfireServiceExtensions.cs`. Maintenance jobs run on a schedule defined there.

## Configuration

See [Queue Configuration](/docs/configuration/queue). Hangfire shares the EF Core SQL Server connection.

## Related

- [Creating Jobs](/docs/queue/creating-jobs)
- [Sending Email](/docs/mail/sending-email)
