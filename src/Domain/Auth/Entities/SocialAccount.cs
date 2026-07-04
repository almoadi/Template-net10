using Template_net10.Domain.Auth.Enums;
using Template_net10.Domain.Common;

namespace Template_net10.Domain.Auth.Entities;

/// <summary>
/// A link between a local <see cref="User"/> and an external identity provider account
/// (the Laravel Socialite analog of a "social account"). One row per provider identity; the
/// combination of <see cref="Provider"/> + <see cref="ProviderUserId"/> is unique. Externally
/// immutable: created through <see cref="Create"/> and mutated only through behaviour methods.
/// </summary>
public class SocialAccount : BaseEntity
{
    // EF Core materialization ctor.
    private SocialAccount()
    {
    }

    public int UserId { get; private set; }

    public SocialProvider Provider { get; private set; }

    /// <summary>Stable user identifier issued by the provider (e.g. Google <c>sub</c>, Entra object id).</summary>
    public string ProviderUserId { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? AvatarUrl { get; private set; }

    /// <summary>Factory: the only way to bring a <see cref="SocialAccount"/> into existence.</summary>
    public static SocialAccount Create(
        int userId,
        SocialProvider provider,
        string providerUserId,
        string email,
        string name,
        string? avatarUrl)
        => new()
        {
            UserId = userId,
            Provider = provider,
            ProviderUserId = providerUserId,
            Email = email.Trim().ToLowerInvariant(),
            Name = name,
            AvatarUrl = avatarUrl,
            IsActive = true,
        };

    /// <summary>Refreshes the cached profile fields from the latest provider payload.</summary>
    public void UpdateProfile(string email, string name, string? avatarUrl)
    {
        Email = email.Trim().ToLowerInvariant();
        Name = name;
        AvatarUrl = avatarUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}
