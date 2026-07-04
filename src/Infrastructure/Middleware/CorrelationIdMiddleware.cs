using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Template_net10.Infrastructure.Middleware;

/// <summary>
/// Ensures every request carries a correlation id. Reads the inbound <c>X-Correlation-ID</c> header
/// (or generates one), echoes it back on the response, aligns <see cref="HttpContext.TraceIdentifier"/>,
/// and opens a logging scope so all logs for the request are tagged. This makes tracing a single
/// request across logs straightforward.
/// </summary>
public sealed class CorrelationIdMiddleware : IMiddleware
{
    public const string HeaderName = "X-Correlation-ID";

    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(ILogger<CorrelationIdMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var value)
            && !string.IsNullOrWhiteSpace(value)
                ? value.ToString()
                : Guid.NewGuid().ToString("n");

        context.TraceIdentifier = correlationId;
        context.Items[HeaderName] = correlationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            await next(context);
        }
    }
}
