namespace Template_net10.Infrastructure.Options;

public enum CacheDriver
{
    Memory,
    Redis,
}

/// <summary>Bound from the <c>Cache</c> section (config/cache.json).</summary>
public sealed class CacheOptions
{
    public const string SectionName = "Cache";

    public CacheDriver Driver { get; set; } = CacheDriver.Memory;

    public string RedisConnection { get; set; } = string.Empty;

    public string InstanceName { get; set; } = string.Empty;

    public int DefaultExpirationMinutes { get; set; } = 5;
}
