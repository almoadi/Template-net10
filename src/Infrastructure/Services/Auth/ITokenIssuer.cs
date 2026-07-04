using Template_net10.Application.Auth.Authentication;

namespace Template_net10.Infrastructure.Services.Auth;

/// <summary>
/// Issues an access/refresh token pair for a user without re-checking credentials. Used by flows that
/// have already authenticated the caller by another means (e.g. two-factor OTP verification).
/// </summary>
public interface ITokenIssuer
{
    Task<AuthTokenDto?> IssueAsync(int userId, CancellationToken cancellationToken = default);
}
