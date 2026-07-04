using Template_net10.Domain.Auth.Enums;

namespace Template_net10.Application.Abstractions.Auth.Social;

/// <summary>
/// Normalized profile returned by a social provider driver after validating an access token
/// (the Laravel Socialite <c>SocialiteUser</c> analog). Provider-specific payloads are mapped
/// into this shape so the rest of the app never depends on a provider's JSON.
/// </summary>
public sealed record SocialUser
{
    public required SocialProvider Provider { get; init; }

    /// <summary>Stable identifier issued by the provider (e.g. Google <c>sub</c>, Entra object id).</summary>
    public required string ProviderUserId { get; init; }

    public required string Email { get; init; }

    public required string Name { get; init; }

    public string? AvatarUrl { get; init; }
}
