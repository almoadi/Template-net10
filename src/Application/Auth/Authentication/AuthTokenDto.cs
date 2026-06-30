namespace Template_net10.Application.Auth.Authentication;

/// <summary>Access token (stateless JWT) plus the server-side session refresh token.</summary>
public sealed class AuthTokenDto
{
    public required string AccessToken { get; init; }

    public required DateTime ExpiresAtUtc { get; init; }

    public required string RefreshToken { get; init; }

    public required DateTime RefreshTokenExpiresAtUtc { get; init; }
}
