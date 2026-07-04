# Caching Overview

Query responses can be cached transparently via the MediatR pipeline.

## ICacheableQuery

Queries that implement `ICacheableQuery` are cached by `CachingBehaviour`:

```csharp
public sealed class GetPermissionsQuery
    : IRequest<ApiResponseDto<IReadOnlyList<PermissionDto>>>, ICacheableQuery
{
    public string CacheKey => "permissions:all";
    public TimeSpan? CacheExpiration => TimeSpan.FromMinutes(10);
}
```

## Cache Drivers

| Driver | Config | Use case |
|--------|--------|----------|
| `Memory` | `Cache:Driver = "Memory"` | Single instance, local dev |
| `Redis` | `Cache:Driver = "Redis"` | Distributed, multi-instance |

See [Cache Configuration](/docs/configuration/cache).

## Invalidation

Cache keys are explicit strings on each query. When mutating data that affects a cached query, either:

- Use a short TTL, or
- Implement cache invalidation in the command handler (future enhancement)

## When & How to Use It

Caching trades a little freshness for a lot of speed. Add `ICacheableQuery` to a query when:

- **The data rarely changes** — the permission catalog, lookup lists, settings, or reference data
  that's read constantly but written rarely.
- **The query is expensive** — heavy joins or aggregations you don't want to run on every request.
- **The same result is requested often** — caching the first response serves everyone else instantly.

When **not** to cache: data that must always be current (account balances, live inventory) or
results that differ per user in ways that make the key hard to manage.

**Scaling tip:** use the `Memory` driver on a single instance for local dev, and switch to
`Redis` when you run multiple instances so they share one cache. Keep TTLs short for data that
can change, and remember the cache key so you can invalidate it after a related write.

## Related

- [MediatR Pipeline](/docs/architecture/mediatr-pipeline)
- [Cache Configuration](/docs/configuration/cache)
