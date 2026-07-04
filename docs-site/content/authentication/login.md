# Login

Authenticate with email and password to receive JWT access and refresh tokens.

## Endpoint

| Method | Path | Permission | Rate limited |
|--------|------|------------|--------------|
| POST | `/api/v1/auth/login` | — (anonymous) | Yes |

## Request Body

```json
{
  "email": "admin@template-net10.local",
  "password": "ChangeMe!123"
}
```

## Success Response (200)

```json
{
  "isSuccess": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "expiresAtUtc": "2026-07-01T12:00:00Z",
    "refreshToken": "abc123...",
    "refreshTokenExpiresAtUtc": "2026-07-31T12:00:00Z"
  }
}
```

## Failure Response (200 with isSuccess false)

Invalid credentials return a failed envelope (not 401) to avoid leaking account existence:

```json
{
  "isSuccess": false,
  "message": "Invalid credentials."
}
```

## Example

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "admin@template-net10.local",
  "password": "ChangeMe!123"
}
```

Use the `accessToken` in subsequent requests:

```http
GET /api/v1/auth/sessions
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

## Implementation

- Command: `LoginCommand` / `LoginHandler`
- Delegates to `Auth.Attempt(email, password)` which creates a `UserSession` and issues tokens
- Controller: `AuthController`

## Related

- [Refresh Tokens](/docs/authentication/refresh-tokens)
- [Rate Limiting](/docs/api/rate-limiting)
- [JWT Configuration](/docs/configuration/jwt)
