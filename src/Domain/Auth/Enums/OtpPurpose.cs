namespace Template_net10.Domain.Auth.Enums;

/// <summary>Why a one-time code/token was issued for a user.</summary>
public enum OtpPurpose
{
    EmailVerification,
    PasswordReset,
    TwoFactor,
}
