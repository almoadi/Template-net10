namespace Template_net10.Application.Abstractions.Time;

/// <summary>
/// Abstraction over the system clock (Laravel: <c>now()</c> / <c>Carbon::now()</c>).
/// Inject this instead of calling <see cref="System.DateTime.UtcNow"/> directly so time-dependent
/// logic in handlers and services stays deterministic and unit-testable.
/// </summary>
public interface IClock
{
    /// <summary>Current instant in UTC. Prefer this for anything persisted or compared.</summary>
    DateTime UtcNow { get; }

    /// <summary>Current instant with offset in UTC.</summary>
    DateTimeOffset UtcNowOffset { get; }

    /// <summary>Today's date (UTC) with the time component set to midnight.</summary>
    DateOnly Today { get; }
}
