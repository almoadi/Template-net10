# Social Login (Socialite)

Sign users in with external identity providers — the .NET analog of
[Laravel Socialite](https://laravel.com/docs/socialite). This template ships **token-based** social
login for **Google** and **Microsoft Entra ID (Azure AD)**, with an extensible driver model so you
can add more providers.

## How the flow works

Social login here is **token-based**, which keeps the backend stateless (no redirect/callback or
client secret to store):

```
1. The front end runs the provider's OAuth/OIDC flow (Google Identity Services,
   MSAL for Entra, …) and receives a provider access token.
2. The front end POSTs that token to POST /api/v1/auth/social/{provider}.
3. The backend validates the token by calling the provider's userinfo endpoint:
     - Google → https://www.googleapis.com/oauth2/v3/userinfo
     - Azure  → https://graph.microsoft.com/v1.0/me
4. The backend finds or provisions a local user, links a SocialAccount,
   and returns this application's own access + refresh tokens.
```

The returned tokens are identical to those from [Login](/docs/authentication/login) — the rest of
the app treats a socially-authenticated user like any other.

## Endpoint

| Method | Path | Permission | Rate limited |
|--------|------|------------|--------------|
| POST | `/api/v1/auth/social/{provider}` | — (anonymous) | Yes |

`{provider}` is `google` or `azure` (case-insensitive).

## Request Body

```json
{
  "accessToken": "ya29.a0Af... (the token issued by the provider)"
}
```

## Success Response (200)

```json
{
  "isSuccess": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "expiresAtUtc": "2026-07-04T12:00:00Z",
    "refreshToken": "abc123...",
    "refreshTokenExpiresAtUtc": "2026-08-03T12:00:00Z"
  }
}
```

## Example

```http
POST /api/v1/auth/social/google
Content-Type: application/json

{
  "accessToken": "ya29.a0Af..."
}
```

## User provisioning

On success the backend resolves the local user in this order:

1. **Existing link** — a `SocialAccount` matching the provider + provider user id is reused (its
   cached name/email/avatar are refreshed).
2. **Match by email** — an existing user with the same email is linked to the new provider.
3. **New user** — a passwordless, **email-verified** user is provisioned via `User.CreateFromSocial`
   and a `SocialAccount` is linked.

Social users have no local password (they cannot sign in via [Login](/docs/authentication/login))
and no phone number. Assign roles/permissions as usual with
[Assign Roles](/docs/users/assign-roles).

## Configuration

Providers are **opt-in**. Enable each one in `config/socialite.json`:

```json
{
  "Socialite": {
    "Google": { "Enabled": true, "ClientId": "..." },
    "Azure":  { "Enabled": true, "ClientId": "..." }
  }
}
```

A disabled or unknown provider returns a failed envelope
(`This social login provider is not enabled.`). See
[Socialite Configuration](/docs/configuration/socialite).

## When & How to Use It

- **Offer "Sign in with Google / Microsoft"** — let users onboard without creating a password.
- **Enterprise SSO with Entra ID** — accept Microsoft work/school accounts via the Azure driver.
- **Link an extra provider to an existing account** — the same email is matched and linked, so a
  user can sign in with either method.

> **Security note:** token-based social login trusts the provider access token. In production,
> configure each provider's `ClientId` and prefer validating the token audience (or verifying a
> signed ID token) so a token minted for a *different* application cannot be replayed against yours.

## Adding a provider

1. Add a value to the `SocialProvider` enum (Domain).
2. Implement `ISocialProviderDriver` in `Infrastructure/Services/Auth/Social/` — it's auto-registered
   by the Scrutor scan.
3. Add the provider to `SocialiteOptions` and `config/socialite.json`.

## Related

- [Login](/docs/authentication/login)
- [Sessions](/docs/authentication/sessions)
- [Socialite Configuration](/docs/configuration/socialite)
