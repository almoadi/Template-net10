using Template_net10.Application.Auth.Authentication;
using Template_net10.Domain.Auth.Enums;

namespace Template_net10.Application.Abstractions.Auth.Social;

/// <summary>
/// Social login facade (the Laravel <c>Socialite</c> analog). Token-based flow: the client obtains
/// an access token from the provider (Google, Azure/Entra) on the front end, then posts it here.
/// The implementation validates the token against the provider, finds or provisions a local user,
/// links the social account, and issues this application's own JWT. Resolve via DI as
/// <see cref="ISocialite"/> or use the static <c>Socialite</c> facade.
/// </summary>
public interface ISocialite
{
    /// <summary>
    /// Validates <paramref name="accessToken"/> with <paramref name="provider"/>, provisions/links the
    /// local user, and returns a freshly issued access/refresh token pair. Throws when the provider is
    /// not configured or the token is invalid. (Laravel: <c>Socialite::driver(...)->userFromToken(...)</c>.)
    /// </summary>
    Task<AuthTokenDto> LoginWithTokenAsync(
        SocialProvider provider, string accessToken, CancellationToken cancellationToken = default);
}
