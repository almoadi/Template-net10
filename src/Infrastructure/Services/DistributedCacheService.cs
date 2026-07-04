using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Template_net10.Application.Abstractions.Caching;
using Template_net10.Infrastructure.Options;

namespace Template_net10.Infrastructure.Services;

/// <summary>
/// <see cref="ICacheService"/> backed by <see cref="IDistributedCache"/> (Memory or Redis, per
/// config/cache.json). Values are stored as UTF-8 JSON. The default expiration comes from
/// <see cref="CacheOptions.DefaultExpirationMinutes"/> when a call does not specify one.
/// </summary>
public sealed class DistributedCacheService : ICacheService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IDistributedCache _cache;
    private readonly TimeSpan _defaultExpiration;

    public DistributedCacheService(IDistributedCache cache, IOptions<CacheOptions> options)
    {
        _cache = cache;
        var minutes = options.Value.DefaultExpirationMinutes > 0 ? options.Value.DefaultExpirationMinutes : 5;
        _defaultExpiration = TimeSpan.FromMinutes(minutes);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var bytes = await _cache.GetAsync(key, cancellationToken);
        return bytes is null or { Length: 0 }
            ? default
            : JsonSerializer.Deserialize<T>(bytes, SerializerOptions);
    }

    public async Task SetAsync<T>(
        string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, SerializerOptions);
        var entryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration,
        };

        await _cache.SetAsync(key, bytes, entryOptions, cancellationToken);
    }

    public async Task<T> RememberAsync<T>(
        string key, Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var value = await factory(cancellationToken);
        await SetAsync(key, value, expiration, cancellationToken);
        return value;
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        => await _cache.GetAsync(key, cancellationToken) is { Length: > 0 };

    public Task ForgetAsync(string key, CancellationToken cancellationToken = default)
        => _cache.RemoveAsync(key, cancellationToken);
}
