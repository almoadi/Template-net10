using Template_net10.Application.Abstractions.Time;

namespace Template_net10.Infrastructure.Services;

/// <summary>
/// Default <see cref="IClock"/> backed by the machine clock, always in UTC. Registered by the
/// Infrastructure Scrutor scan. Swap for a fixed/fake clock in tests to control time.
/// </summary>
public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;

    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
