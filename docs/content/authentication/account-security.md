# Email Verification & Two-Factor

Optional account-security flows — **email verification**, **password reset**, and **two-factor login
(email OTP)**. Everything is off by default and toggled in [`config/auth.json`](/docs/configuration/auth),
so the base login flow is unchanged until you opt in.

One-time codes are **never stored in the database** — they live as SHA-256 hashes in the distributed
cache ([Memory or Redis](/docs/configuration/cache)) and expire automatically. Email bodies come from
HTML templates, not inline markup.

## Toggles

| `config/auth.json` key | Effect |
|------------------------|--------|
| `RequireEmailVerification` | Block login until the user verifies their email |
| `TwoFactorEnabled` | Require an email OTP as a second login step for everyone |
| `OtpLength`, `OtpExpiryMinutes` | Two-factor code size / lifetime |
| `EmailVerificationExpiryHours` | Verification token lifetime |
| `PasswordResetExpiryMinutes` | Reset token lifetime |

Two-factor can also be enabled per user via `User.EnableTwoFactor()` even when the global flag is off.

## Endpoints

All live on `AccountController` (`/api/v1/auth`), are `[AllowAnonymous]`, and rate-limited like login.

| Method | Route | Purpose |
|--------|-------|---------|
| POST | `/api/v1/auth/email/verify/request` | Send an email-verification token |
| POST | `/api/v1/auth/email/verify` | Verify with `{ email, token }` |
| POST | `/api/v1/auth/password/forgot` | Send a password-reset token |
| POST | `/api/v1/auth/password/reset` | Reset with `{ email, token, newPassword }` |
| POST | `/api/v1/auth/2fa/verify` | Complete 2FA login with `{ email, code }` |

`request`/`forgot` always return success so callers cannot enumerate which emails exist.

## Login flow with two-factor

When 2FA applies to a user, `POST /api/v1/auth/login` validates the password, emails an OTP, and returns
**400** with `TwoFactorRequired`. The client then completes sign-in:

```http
POST /api/v1/auth/login           { "email": "...", "password": "..." }
→ 400 "A verification code has been sent to your email."

POST /api/v1/auth/2fa/verify      { "email": "...", "code": "123456" }
→ 200 { accessToken, refreshToken, ... }
```

`ResetPassword` also revokes every active session, so old refresh tokens stop working immediately.

## How it works

- **`IAccountService`** (Application) orchestrates the flows; the login gate is
  `EnsureLoginAllowedAsync`, called from `LoginCommandHandler` (no-op when flags are off).
- **Codes** are generated, hashed (SHA-256), and stored in `ICacheService` under `otp:{purpose}:{userId}`
  with a TTL. Verification is a fixed-time hash compare followed by a single-use delete.
- **Durable state** — `EmailVerifiedAt` and `TwoFactorEnabled` — lives on the `User` row (a migration
  adds those two columns).
- **Two-factor verify** issues the same JWT + refresh session as a normal login via a shared `ITokenIssuer`.

## Email templates

Bodies are rendered by `IEmailTemplateRenderer` from HTML files under
`src/API/resources/email-templates/` (`email-verification.html`, `password-reset.html`,
`two-factor.html`). Templates use `{{appName}}`, `{{title}}`, `{{intro}}`, and `{{code}}` placeholders;
the title/intro text stays in the [YAML lang files](/docs/localization/overview). Edit the HTML to
match your brand — no Razor engine or redeploy of code required.

## Related

- [Auth Configuration](/docs/configuration/auth)
- [Login](/docs/authentication/login)
- [Cache Configuration](/docs/configuration/cache)
