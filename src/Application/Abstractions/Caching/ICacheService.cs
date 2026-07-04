namespace Template_net10.Application.Abstractions.Caching;

/// <summary>
/// General-purpose cache abstraction (Laravel: the <c>Cache</c> facade). Backed by the configured
/// distributed cache (Memory or Redis — see config/cache.json). Values are JSON-serialized.
/// For read-model caching driven by MediatR, prefer <see cref="ICacheableQuery"/> + the caching behaviour;
/// use this service for ad-hoc caching inside handlers, services, and jobs.
/// </summary>
public interface ICacheService
{
    /// <summary>Gets a cached value, or <c>default</c> when the key is missing/expired. (Laravel: <c>Cache::get</c>)</summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>Stores a value with an optional expiry (falls back to the configured default). (Laravel: <c>Cache::put</c>)</summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>Returns the cached value, or computes, stores, and returns it. (Laravel: <c>Cache::remember</c>)</summary>
    Task<T> RememberAsync<T>(
        string key, Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>True when a value exists for the key. (Laravel: <c>Cache::has</c>)</summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Removes a cached value. Safe when the key is absent. (Laravel: <c>Cache::forget</c>)</summary>
    Task ForgetAsync(string key, CancellationToken cancellationToken = default);
}
