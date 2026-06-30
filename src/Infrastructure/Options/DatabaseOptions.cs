namespace Template_net10.Infrastructure.Options;

/// <summary>Bound from the <c>Database</c> section (config/database.json).</summary>
public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string ConnectionString { get; set; } = string.Empty;

    public bool EnableDetailedErrors { get; set; }

    public bool EnableSensitiveDataLogging { get; set; }

    public int CommandTimeoutSeconds { get; set; } = 30;

    public int MaxRetryCount { get; set; } = 3;
}
