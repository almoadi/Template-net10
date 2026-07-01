using Template_net10.Domain.Common;

namespace Template_net10.Domain.Auth.Entities;

/// <summary>
/// A server-side authentication session created on login. Backs refresh-token rotation and
/// revocation while the access token itself stays a stateless JWT. Externally immutable:
/// created through <see cref="Create"/> and mutated only through behaviour methods.
/// </summary>
public class UserSession : BaseEntity
{
    // EF Core materialization ctor.
    private UserSession()
    {
    }

    public int UserId { get; private set; }

    /// <summary>SHA-256 hash of the refresh token; the raw token is never persisted.</summary>
    public string RefreshTokenHash { get; private set; } = string.Empty;

    public DateTime ExpiresAt { get; private set; }

    public DateTime LastActivityAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    /// <summary>Optional client-supplied device name (e.g. from an <c>X-Device-Name</c> header).</summary>
    public string? Device { get; private set; }

    public string? IpAddress { get; private set; }

    public string? UserAgent { get; private set; }

    /// <summary>True while the session is neither revoked nor expired.</summary>
    public new bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;

    /// <summary>Factory: the only way to bring a <see cref="UserSession"/> into existence.</summary>
    public static UserSession Create(
        int userId,
        string refreshTokenHash,
        DateTime expiresAtUtc,
        string? device,
        string? ipAddress,
        string? userAgent)
        => new()
        {
            UserId = userId,
            RefreshTokenHash = refreshTokenHash,
            ExpiresAt = expiresAtUtc,
            LastActivityAt = DateTime.UtcNow,
            Device = device,
            IpAddress = ipAddress,
            UserAgent = userAgent,
        };

    /// <summary>Rotates the refresh token (new hash + expiry) and records fresh activity.</summary>
    public void Rotate(string refreshTokenHash, DateTime expiresAtUtc)
    {
        RefreshTokenHash = refreshTokenHash;
        ExpiresAt = expiresAtUtc;
        LastActivityAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Revokes the session so its refresh token can no longer be used.</summary>
    public void Revoke()
    {
        if (RevokedAt is null)
        {
            RevokedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
