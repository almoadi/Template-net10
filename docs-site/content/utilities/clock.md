# Clock

Inject `IClock` instead of calling `DateTime.UtcNow` directly so time-dependent logic in handlers and
services stays deterministic and unit-testable — the .NET analog of Laravel's `now()`.

## IClock

```csharp
public interface IClock
{
    DateTime UtcNow { get; }
    DateTimeOffset UtcNowOffset { get; }
    DateOnly Today { get; }
}
```

The default `SystemClock` reads the machine clock in UTC and is registered automatically.

## Usage

```csharp
public sealed class ExpireTokensHandler(IApplicationDbContext context, IClock clock)
{
    public async Task Handle(ExpireTokensCommand command, CancellationToken ct)
    {
        var cutoff = clock.UtcNow.AddDays(-30);
        // ... compare against cutoff
    }
}
```

## Testing

Swap in a fixed clock to make time deterministic:

```csharp
var clock = new Mock<IClock>();
clock.SetupGet(c => c.UtcNow).Returns(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
```

> Domain entities set their own audit timestamps (`CreatedAt`, `UpdatedAt`) directly, since the
> Domain layer depends on nothing. Use `IClock` in the Application/Infrastructure layers.

## Related

- [Clean Architecture & CQRS](/docs/architecture/clean-architecture)
- [Testing Overview](/docs/testing/overview)
