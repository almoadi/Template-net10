using Template_net10.Application.Auth.Authentication;

namespace Template_net10.Application.Abstractions.Account;

/// <summary>
/// Optional account-security flows (email verification, password reset, two-factor login). All are
/// gated by <c>config/auth.json</c> and are no-ops in effect until enabled. Implemented in Infrastructure.
/// </summary>
public interface IAccountService
{
    /// <summary>True when a second factor is required for every login.</summary>
    bool TwoFactorRequired { get; }

    /// <summary>
    /// Throws when the (validated) user is not allowed to complete a normal login — e.g. email is
    /// unverified and verification is required, or two-factor is enabled (an OTP is sent and the caller
    /// must finish via <see cref="VerifyTwoFactorAsync"/>). Silent when credentials are invalid.
    /// </summary>
    Task EnsureLoginAllowedAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>Issues and emails an email-verification token. Silent when the email is unknown.</summary>
    Task SendEmailVerificationAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Verifies an email-verification token. Returns false when invalid/expired.</summary>
    Task<bool> VerifyEmailAsync(string email, string token, CancellationToken cancellationToken = default);

    /// <summary>Issues and emails a password-reset token. Silent when the email is unknown (no enumeration).</summary>
    Task SendPasswordResetAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Resets the password using a reset token and revokes all sessions. Returns false when invalid/expired.</summary>
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>Completes a two-factor login by verifying the emailed OTP. Returns tokens, or null when invalid.</summary>
    Task<AuthTokenDto?> VerifyTwoFactorAsync(string email, string code, CancellationToken cancellationToken = default);
}
