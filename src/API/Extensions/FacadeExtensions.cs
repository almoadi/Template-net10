using Microsoft.AspNetCore.Http;
using Template_net10.Application.Abstractions.Caching;
using Template_net10.Application.Abstractions.Encryption;
using Template_net10.Application.Abstractions.Features;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Abstractions.Storage;
using Template_net10.Application.Common.Facades;

namespace Template_net10.API.Extensions;

public static class FacadeExtensions
{
    /// <summary>
    /// Initializes every static facade (<c>Auth</c>, <c>Cache</c>, <c>Storage</c>, <c>Feature</c>,
    /// <c>Crypt</c>, <c>Hash</c>) so each resolves its request-scoped service from the current
    /// <see cref="HttpContext"/>. Call once after the app is built. Prefer injecting the underlying
    /// interfaces; facades exist for Laravel-like ergonomics.
    /// </summary>
    public static WebApplication UseFacades(this WebApplication app)
    {
        var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();

        Auth.SetResolver(() => Resolve<IAuth>(httpContextAccessor));
        Cache.SetResolver(() => Resolve<ICacheService>(httpContextAccessor));
        Storage.SetResolver(() => Resolve<IStorage>(httpContextAccessor));
        Feature.SetResolver(() => Resolve<IFeatureFlags>(httpContextAccessor));
        Crypt.SetResolver(() => Resolve<IEncryptor>(httpContextAccessor));
        Hash.SetResolver(() => Resolve<IPasswordHasher>(httpContextAccessor));

        return app;
    }

    private static T Resolve<T>(IHttpContextAccessor httpContextAccessor) where T : notnull
    {
        var requestServices = httpContextAccessor.HttpContext?.RequestServices
            ?? throw new InvalidOperationException(
                $"The {typeof(T).Name} facade can only be used within an HTTP request. Inject {typeof(T).Name} instead.");

        return requestServices.GetRequiredService<T>();
    }
}
