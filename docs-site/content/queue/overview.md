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

## When & How to Use It

Background jobs let you move slow or scheduled work off the request thread so users never wait.
Reach for the queue when:

- **A task is slow** — sending email, generating a PDF/report, resizing images, or calling a
  third-party API. Enqueue it and return the response immediately.
- **Work should happen later** — a delayed reminder "in 24 hours" or a follow-up message.
- **Something runs on a schedule** — nightly cleanup, expiring old sessions, daily digests. These
  are recurring jobs registered at startup.
- **You need automatic retries** — Hangfire re-runs failed jobs, which is safer than doing fragile
  work inline during a request.

**Typical flow:** define a job interface, implement it in Infrastructure, then call
`_jobScheduler.Enqueue<IEmailJob>(j => j.SendAsync(...))`. Watch and manage runs on the
`/hangfire` dashboard.

## Related

- [Creating Jobs](/docs/queue/creating-jobs)
- [Sending Email](/docs/mail/sending-email)
