# Cache Service

Beyond query caching via [`ICacheableQuery`](/docs/caching/overview), the template ships a
general-purpose `Cache` facade (Laravel's `Cache`) for ad-hoc caching inside handlers, services, and jobs.

## ICacheService

```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default);
    Task<T> RememberAsync<T>(string key, Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null, CancellationToken ct = default);
    Task<bool> ExistsAsync(string key, CancellationToken ct = default);
    Task ForgetAsync(string key, CancellationToken ct = default);
}
```

Values are JSON-serialized and stored in the configured distributed cache (Memory or Redis). When no
expiration is passed, `Cache:DefaultExpirationMinutes` from `config/cache.json` is used.

## Remember pattern

`RememberAsync` returns the cached value or computes, stores, and returns it in one call:

```csharp
var stats = await cache.RememberAsync(
    "dashboard:stats",
    async ct => await BuildStatsAsync(ct),
    TimeSpan.FromMinutes(10),
    cancellationToken);
```

## Cache facade

```csharp
var user = await Cache.Remember($"user:{id}", ct => LoadUserAsync(id, ct), TimeSpan.FromMinutes(5));
await Cache.Forget($"user:{id}"); // invalidate after an update
```

## Related

- [Caching Overview](/docs/caching/overview)
- [Cache Configuration](/docs/configuration/cache)
