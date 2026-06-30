using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Template_net10.Application.Abstractions.Caching;

namespace Template_net10.Application.Behaviours;

/// <summary>
/// Caches responses for requests that opt in via <see cref="ICacheableQuery"/>.
/// Non-cacheable requests pass straight through.
/// </summary>
public sealed class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingBehaviour<TRequest, TResponse>> _logger;

    public CachingBehaviour(IMemoryCache cache, ILogger<CachingBehaviour<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICacheableQuery cacheable)
        {
            return await next(cancellationToken);
        }

        if (_cache.TryGetValue(cacheable.CacheKey, out TResponse? cached) && cached is not null)
        {
            _logger.LogInformation("Cache hit for {CacheKey}", cacheable.CacheKey);
            return cached;
        }

        var response = await next(cancellationToken);

        _cache.Set(cacheable.CacheKey, response, cacheable.Expiration);
        return response;
    }
}
