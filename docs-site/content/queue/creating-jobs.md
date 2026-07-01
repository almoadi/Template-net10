# Creating Jobs

Add background work using the `IJobScheduler` abstraction and Hangfire.

## Enqueue a Job

```csharp
public sealed class MyHandler(IJobScheduler jobScheduler)
{
    public async Task Handle(MyCommand command, CancellationToken ct)
    {
        // ... save to DB ...

        jobScheduler.Enqueue<IEmailJob>(j => j.SendAsync(new EmailMessage
        {
            To = command.Email,
            Subject = "Notification",
            Body = "Your request was processed.",
        }));
    }
}
```

## Define a Job Interface

In `Application/Abstractions/Jobs/`:

```csharp
public interface IMyJob
{
    Task RunAsync(int entityId, CancellationToken ct = default);
}
```

## Implement the Job

In `Infrastructure/Services/` (auto-registered by Scrutor):

```csharp
public sealed class MyJob(IApplicationDbContext context) : IMyJob
{
    public async Task RunAsync(int entityId, CancellationToken ct = default)
    {
        // background work here
    }
}
```

## Job Types

| Method | Use case |
|--------|----------|
| `Enqueue` | Run as soon as a worker is free |
| `Schedule` | Run after a delay |
| `Recurring` | Cron-based schedule (registered at startup) |

## Existing Jobs

| Job | Purpose |
|-----|---------|
| `IEmailJob` | Send a single email asynchronously |
| `IMaintenanceJob` | Scheduled cleanup / maintenance tasks |

## Related

- [Queue Overview](/docs/queue/overview)
- [Extending the Template](/docs/architecture/extending-the-template)
