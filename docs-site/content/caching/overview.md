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

## Related

- [MediatR Pipeline](/docs/architecture/mediatr-pipeline)
- [Cache Configuration](/docs/configuration/cache)
