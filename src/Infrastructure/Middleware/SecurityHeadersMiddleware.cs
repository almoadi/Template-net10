using Microsoft.AspNetCore.Http;

namespace Template_net10.Infrastructure.Middleware;

/// <summary>
/// Adds a baseline set of security response headers to every response. These are safe defaults for
/// a JSON API; tighten or extend them per deployment needs. HSTS is intentionally left to
/// <c>UseHttpsRedirection</c>/host configuration.
/// </summary>
public sealed class SecurityHeadersMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "no-referrer";
            headers["X-Permitted-Cross-Domain-Policies"] = "none";
            headers["Cross-Origin-Opener-Policy"] = "same-origin";

            // Remove headers that leak server implementation details.
            headers.Remove("X-Powered-By");
            headers.Remove("Server");

            return Task.CompletedTask;
        });

        await next(context);
    }
}
