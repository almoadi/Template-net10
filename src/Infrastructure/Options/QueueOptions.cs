namespace Template_net10.Infrastructure.Options;

public enum QueueDriver
{
    /// <summary>SQL Server-backed background jobs via Hangfire.</summary>
    Hangfire,
}

/// <summary>Bound from the <c>Queue</c> section (config/queue.json).</summary>
public sealed class QueueOptions
{
    public const string SectionName = "Queue";

    public QueueDriver Driver { get; set; } = QueueDriver.Hangfire;

    public bool DashboardEnabled { get; set; } = true;

    public string DashboardPath { get; set; } = "/hangfire";

    public int WorkerCount { get; set; }

    public string[] Queues { get; set; } = ["default"];

    public string SchemaName { get; set; } = "HangFire";
}
