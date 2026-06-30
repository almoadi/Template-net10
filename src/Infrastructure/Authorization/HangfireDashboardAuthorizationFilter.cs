using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;

namespace Template_net10.Infrastructure.Authorization;

/// <summary>
/// Guards the Hangfire dashboard. Open in Development for convenience; in other environments it
/// requires an authenticated user. Tighten further (e.g. require a permission) for production.
/// </summary>
public sealed class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly bool _isDevelopment;

    public HangfireDashboardAuthorizationFilter(bool isDevelopment) => _isDevelopment = isDevelopment;

    public bool Authorize([NotNull] DashboardContext context)
    {
        if (_isDevelopment)
        {
            return true;
        }

        var httpContext = context.GetHttpContext();
        return httpContext.User.Identity?.IsAuthenticated ?? false;
    }
}
