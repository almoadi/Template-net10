using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Template_net10.Application.Abstractions.Caching;
using Template_net10.Infrastructure.Options;

namespace Template_net10.Infrastructure.Middleware;

/// <summary>
/// Makes mutating requests (POST/PUT/PATCH/DELETE) safe to retry. When a client sends an
/// <c>Idempotency-Key</c> header, the first successful response is cached and replayed for any
/// repeat with the same key — so a retried payment/order does not run twice. Backed by the
/// configured distributed cache via <see cref="ICacheService"/>.
/// </summary>
public sealed class IdempotencyMiddleware : IMiddleware
{
    private readonly ICacheService _cache;
    private readonly IdempotencyOptions _options;

    public IdempotencyMiddleware(ICacheService cache, IOptions<IdempotencyOptions> options)
    {
        _cache = cache;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!_options.Enabled
            || !IsMutating(context.Request.Method)
            || !context.Request.Headers.TryGetValue(_options.HeaderName, out var header)
            || string.IsNullOrWhiteSpace(header))
        {
            await next(context);
            return;
        }

        var cacheKey = $"idempotency:{header}";
        var replay = await _cache.GetAsync<CachedResponse>(cacheKey, context.RequestAborted);
        if (replay is not null)
        {
            context.Response.StatusCode = replay.StatusCode;
            context.Response.ContentType = replay.ContentType;
            context.Response.Headers["Idempotency-Replayed"] = "true";
            await context.Response.WriteAsync(replay.Body, context.RequestAborted);
            return;
        }

        // Buffer the response so it can be both sent and cached.
        var originalBody = context.Response.Body;
        using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        try
        {
            await next(context);

            buffer.Position = 0;
            var body = await new StreamReader(buffer).ReadToEndAsync(context.RequestAborted);
            buffer.Position = 0;
            await buffer.CopyToAsync(originalBody, context.RequestAborted);

            if (context.Response.StatusCode is >= 200 and < 300)
            {
                var record = new CachedResponse(
                    context.Response.StatusCode,
                    context.Response.ContentType ?? "application/json",
                    body);

                await _cache.SetAsync(
                    cacheKey, record, TimeSpan.FromMinutes(_options.ExpirationMinutes), context.RequestAborted);
            }
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private static bool IsMutating(string method)
        => HttpMethods.IsPost(method)
        || HttpMethods.IsPut(method)
        || HttpMethods.IsPatch(method)
        || HttpMethods.IsDelete(method);

    public sealed record CachedResponse(int StatusCode, string ContentType, string Body);
}
