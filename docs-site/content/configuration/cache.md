# Cache Configuration

Distributed cache driver settings.

**File:** `src/API/config/cache.json`  
**Options class:** `CacheOptions`

## Settings

| Key | Default | Description |
|-----|---------|-------------|
| `Cache:Driver` | `Memory` | `Memory` or `Redis` |
| `Cache:RedisConnection` | `""` | Redis connection string (required for Redis) |
| `Cache:InstanceName` | `Template-net10:` | Key prefix for Redis entries |
| `Cache:DefaultExpirationMinutes` | `5` | Default TTL for cached queries |

## Drivers

| Driver | Use case |
|--------|----------|
| `Memory` | Local development, single instance |
| `Redis` | Production, multiple API instances |

## Usage

Queries implementing `ICacheableQuery` are cached automatically by `CachingBehaviour`. See [Caching Overview](/docs/caching/overview).

## Related

- [Configuration Overview](/docs/configuration/overview)
