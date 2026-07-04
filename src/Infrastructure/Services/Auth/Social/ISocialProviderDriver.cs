using Template_net10.Application.Abstractions.Auth.Social;
using Template_net10.Domain.Auth.Enums;

namespace Template_net10.Infrastructure.Services.Auth.Social;

/// <summary>
/// A single social provider integration. Each driver knows how to turn a provider access token into
/// a normalized <see cref="SocialUser"/>. Implementations are discovered by DI and selected by
/// <see cref="Provider"/> (the Laravel Socialite "driver" analog).
/// </summary>
public interface ISocialProviderDriver
{
    SocialProvider Provider { get; }

    /// <summary>Validates the token against the provider and returns the caller's profile, or throws when invalid.</summary>
    Task<SocialUser> GetUserAsync(string accessToken, CancellationToken cancellationToken = default);
}
