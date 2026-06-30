using System.Linq.Expressions;
using Hangfire;
using Template_net10.Application.Abstractions.Jobs;

namespace Template_net10.Infrastructure.Jobs;

/// <summary>Hangfire-backed implementation of <see cref="IJobScheduler"/>.</summary>
public sealed class HangfireJobScheduler : IJobScheduler
{
    private readonly IBackgroundJobClient _backgroundJobs;
    private readonly IRecurringJobManager _recurringJobs;

    public HangfireJobScheduler(
        IBackgroundJobClient backgroundJobs, IRecurringJobManager recurringJobs)
    {
        _backgroundJobs = backgroundJobs;
        _recurringJobs = recurringJobs;
    }

    public string Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall) where TJob : notnull
        => _backgroundJobs.Enqueue(methodCall);

    public string Schedule<TJob>(Expression<Func<TJob, Task>> methodCall, TimeSpan delay)
        where TJob : notnull
        => _backgroundJobs.Schedule(methodCall, delay);

    public void AddOrUpdateRecurring<TJob>(
        string recurringJobId, Expression<Func<TJob, Task>> methodCall, string cronExpression)
        where TJob : notnull
        => _recurringJobs.AddOrUpdate(recurringJobId, methodCall, cronExpression);
}
