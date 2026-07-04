using Template_net10.Application.Abstractions.Caching;

namespace Template_net10.Application.Common.Facades;

/// <summary>
/// Laravel-style static <c>Cache</c> facade. Proxies to the request-scoped <see cref="ICacheService"/>
/// resolved through a delegate set at startup (<see cref="SetResolver"/>). Prefer injecting
/// <see cref="ICacheService"/> in handlers/services; use this facade for Laravel-like ergonomics
/// (<c>Cache.Remember(...)</c>, <c>Cache.Get(...)</c>). Only valid inside an HTTP request scope.
/// </summary>
public static class Cache
{
    private static Func<ICacheService>? _resolver;

    /// <summary>Wires the facade to the DI container. Called once during application startup.</summary>
    public static void SetResolver(Func<ICacheService> resolver) => _resolver = resolver;

    private static ICacheService Instance => (_resolver ?? throw new InvalidOperationException(
        "Cache facade is not initialized. Call app.UseFacades() during startup."))();

    public static Task<T?> Get<T>(string key, CancellationToken cancellationToken = default)
        => Instance.GetAsync<T>(key, cancellationToken);

    public static Task Put<T>(
        string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        => Instance.SetAsync(key, value, expiration, cancellationToken);

    public static Task<T> Remember<T>(
        string key, Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        => Instance.RememberAsync(key, factory, expiration, cancellationToken);

    public static Task<bool> Has(string key, CancellationToken cancellationToken = default)
        => Instance.ExistsAsync(key, cancellationToken);

    public static Task Forget(string key, CancellationToken cancellationToken = default)
        => Instance.ForgetAsync(key, cancellationToken);
}
