# Refresh Tokens

When the access token expires, exchange a valid refresh token for a new access token and a rotated refresh token.

## Endpoint

| Method | Path | Permission | Rate limited |
|--------|------|------------|--------------|
| POST | `/api/v1/auth/refresh` | — (anonymous) | Yes |

## Request Body

```json
{
  "refreshToken": "your-current-refresh-token"
}
```

## Success Response (200)

Returns a new `AuthTokenDto` with a fresh access token and a **new** refresh token. The old refresh token is invalidated (rotation).

```json
{
  "isSuccess": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "expiresAtUtc": "2026-07-01T13:00:00Z",
    "refreshToken": "new-rotated-token...",
    "refreshTokenExpiresAtUtc": "2026-07-31T12:00:00Z"
  }
}
```

## Failure

Invalid or expired refresh tokens return `isSuccess: false`.

## How Rotation Works

1. Client sends current refresh token.
2. Server finds the `UserSession` by hashed token lookup.
3. Server validates expiry and revocation status.
4. Server issues new access + refresh tokens.
5. Old refresh token is replaced — reuse of old token revokes the session.

## Configuration

Refresh token lifetime is controlled by `Jwt:RefreshTokenExpiryDays` in `config/jwt.json` (default: 30 days).

## Related

- [Login](/docs/authentication/login)
- [Logout](/docs/authentication/logout)
- [Sessions](/docs/authentication/sessions)
