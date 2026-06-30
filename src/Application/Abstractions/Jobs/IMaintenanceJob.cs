namespace Template_net10.Application.Abstractions.Jobs;

/// <summary>
/// Sample maintenance job, registered as a recurring Hangfire job at startup to demonstrate
/// scheduled background work. Replace/extend with real periodic tasks (cleanup, digests, …).
/// </summary>
public interface IMaintenanceJob
{
    Task HeartbeatAsync();
}
