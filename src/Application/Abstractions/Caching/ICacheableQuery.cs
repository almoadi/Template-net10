namespace Template_net10.Application.Abstractions.Caching;

/// <summary>
/// Marker for queries whose result should be cached by the <c>CachingBehaviour</c>.
/// Implement on a query to opt it into the cache pipeline.
/// </summary>
public interface ICacheableQuery
{
    /// <summary>Unique cache key for this request instance (include any filter values).</summary>
    string CacheKey { get; }

    /// <summary>How long the cached entry stays valid.</summary>
    TimeSpan Expiration { get; }
}
