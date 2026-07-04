# Socialite Configuration

Social login (Laravel Socialite style) provider settings.

**File:** `src/API/config/socialite.json`  
**Options class:** `SocialiteOptions`

## Settings

| Key | Default | Description |
|-----|---------|-------------|
| `Socialite:Google:Enabled` | `false` | Allow Google sign-in |
| `Socialite:Google:ClientId` | _(empty)_ | Optional Google OAuth client id (audience validation / documentation) |
| `Socialite:Azure:Enabled` | `false` | Allow Microsoft Entra ID (Azure AD) sign-in |
| `Socialite:Azure:ClientId` | _(empty)_ | Optional Entra application (client) id |

Providers are **opt-in** — a provider with `Enabled: false` (the default) is rejected even though a
driver exists.

## Example

```json
{
  "Socialite": {
    "Google": { "Enabled": true, "ClientId": "1234567890-abc.apps.googleusercontent.com" },
    "Azure":  { "Enabled": true, "ClientId": "00000000-0000-0000-0000-000000000000" }
  }
}
```

> **Secrets:** the token-based flow does not require a client secret on the backend. Any real secrets
> should still come from `dotnet user-secrets`, environment variables (`Socialite__Azure__ClientId`),
> or a vault — never committed.

## Usage

Post a provider access token to `POST /api/v1/auth/social/{provider}`. See
[Social Login](/docs/authentication/social-login).

## Related

- [Social Login](/docs/authentication/social-login)
- [Configuration Overview](/docs/configuration/overview)
