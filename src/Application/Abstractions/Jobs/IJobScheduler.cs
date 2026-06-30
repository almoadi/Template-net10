using System.Linq.Expressions;

namespace Template_net10.Application.Abstractions.Jobs;

/// <summary>
/// Abstraction over the background-job engine (implemented with Hangfire in Infrastructure).
/// Handlers depend on this — never on Hangfire directly — so the queue technology can be swapped.
/// </summary>
public interface IJobScheduler
{
    /// <summary>Fire-and-forget: run as soon as a worker is free. Returns the job id.</summary>
    string Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall) where TJob : notnull;

    /// <summary>Run once after <paramref name="delay"/>. Returns the job id.</summary>
    string Schedule<TJob>(Expression<Func<TJob, Task>> methodCall, TimeSpan delay) where TJob : notnull;

    /// <summary>Create or update a recurring job identified by <paramref name="recurringJobId"/>.</summary>
    void AddOrUpdateRecurring<TJob>(
        string recurringJobId, Expression<Func<TJob, Task>> methodCall, string cronExpression)
        where TJob : notnull;
}
