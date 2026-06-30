namespace Template_net10.Application.Auth.Authentication;

/// <summary>Access token issued on a successful login.</summary>
public sealed class AuthTokenDto
{
    public required string AccessToken { get; init; }

    public required DateTime ExpiresAtUtc { get; init; }
}
