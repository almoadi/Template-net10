namespace Template_net10.Infrastructure.Options;

/// <summary>Bound from the <c>Auth</c> section (config/auth.json). All flags default to off.</summary>
public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    /// <summary>When true, users must verify their email before they can log in.</summary>
    public bool RequireEmailVerification { get; set; }

    /// <summary>When true, every login requires a second factor (email OTP).</summary>
    public bool TwoFactorEnabled { get; set; }

    /// <summary>Number of digits in a two-factor OTP code.</summary>
    public int OtpLength { get; set; } = 6;

    /// <summary>How long a two-factor OTP stays valid.</summary>
    public int OtpExpiryMinutes { get; set; } = 10;

    /// <summary>How long an email-verification token stays valid.</summary>
    public int EmailVerificationExpiryHours { get; set; } = 24;

    /// <summary>How long a password-reset token stays valid.</summary>
    public int PasswordResetExpiryMinutes { get; set; } = 30;
}
