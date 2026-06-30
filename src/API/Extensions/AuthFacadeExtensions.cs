using Microsoft.AspNetCore.Http;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Common.Facades;

namespace Template_net10.API.Extensions;

public static class AuthFacadeExtensions
{
    /// <summary>
    /// Initializes the static <c>Auth</c> facade so it resolves the request-scoped <see cref="IAuth"/>
    /// from the current <see cref="HttpContext"/>. Call once after the app is built.
    /// </summary>
    public static WebApplication UseAuthFacade(this WebApplication app)
    {
        var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();

        Auth.SetResolver(() =>
        {
            var requestServices = httpContextAccessor.HttpContext?.RequestServices
                ?? throw new InvalidOperationException(
                    "The Auth facade can only be used within an HTTP request. Inject IAuth instead.");

            return requestServices.GetRequiredService<IAuth>();
        });

        return app;
    }
}
