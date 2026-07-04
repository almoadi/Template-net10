# Auth Configuration

Optional account-security settings. Every flag is **off by default** — turning one on is opt-in and
does not change the base login flow.

**File:** `src/API/config/auth.json`  
**Options class:** `AuthOptions`

## Settings

| Key | Default | Description |
|-----|---------|-------------|
| `Auth:RequireEmailVerification` | `false` | Block login until the user verifies their email |
| `Auth:TwoFactorEnabled` | `false` | Require an email OTP as a second login step |
| `Auth:OtpLength` | `6` | Number of digits in a two-factor OTP |
| `Auth:OtpExpiryMinutes` | `10` | Two-factor OTP lifetime |
| `Auth:EmailVerificationExpiryHours` | `24` | Email-verification token lifetime |
| `Auth:PasswordResetExpiryMinutes` | `30` | Password-reset token lifetime |

## Notes

- One-time codes are stored (hashed) in the distributed [cache](/docs/configuration/cache), not the
  database, and expire automatically.
- These settings drive the flows documented in
  [Email Verification & Two-Factor](/docs/authentication/account-security).

## Related

- [Email Verification & Two-Factor](/docs/authentication/account-security)
- [Configuration Overview](/docs/configuration/overview)
