using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Template_net10.API.Extensions;

public static class RateLimitingPolicies
{
    public const string Auth = "auth";
}

public static class AuthRateLimitingExtensions
{
    /// <summary>Throttles authentication endpoints to slow credential-stuffing attempts.</summary>
    public static IServiceCollection AddAuthRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddFixedWindowLimiter(RateLimitingPolicies.Auth, limiter =>
            {
                limiter.PermitLimit = 10;
                limiter.Window = TimeSpan.FromMinutes(1);
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiter.QueueLimit = 0;
            });
        });

        return services;
    }
}
