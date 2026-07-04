using Template_net10.Application.Abstractions.Auth.Social;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Domain.Auth.Enums;

namespace Template_net10.Application.Common.Facades;

/// <summary>
/// Laravel-style static <c>Socialite</c> facade. Proxies to the request-scoped <see cref="ISocialite"/>
/// resolved through a delegate set at startup (<see cref="SetResolver"/>). Prefer injecting
/// <see cref="ISocialite"/> in handlers/services; use this facade for Laravel-like ergonomics
/// (<c>Socialite.Login(...)</c>). Only valid inside an HTTP request scope.
/// </summary>
public static class Socialite
{
    private static Func<ISocialite>? _resolver;

    /// <summary>Wires the facade to the DI container. Called once during application startup.</summary>
    public static void SetResolver(Func<ISocialite> resolver) => _resolver = resolver;

    private static ISocialite Instance => (_resolver ?? throw new InvalidOperationException(
        "Socialite facade is not initialized. Call app.UseFacades() during startup."))();

    /// <summary>Exchanges a provider access token for this application's JWT. (Laravel: <c>Socialite::driver(...)->userFromToken(...)</c>.)</summary>
    public static Task<AuthTokenDto> Login(
        SocialProvider provider, string accessToken, CancellationToken cancellationToken = default)
        => Instance.LoginWithTokenAsync(provider, accessToken, cancellationToken);
}
