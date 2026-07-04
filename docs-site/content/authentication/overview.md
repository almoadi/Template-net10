# Authentication Overview

Template-net10 uses **JWT access tokens** combined with **server-side refresh-token sessions**. Login is by **email + password**.

## Authentication Flow

```
1. Client POST /api/auth/login (email + password)
2. Server validates credentials, creates UserSession row (hashed refresh token)
3. Server returns AuthTokenDto:
   - accessToken (JWT, short-lived)
   - refreshToken (opaque, long-lived)
4. Client sends accessToken as Bearer token on protected requests
5. When access token expires, POST /api/auth/refresh with refreshToken
6. Server rotates refresh token, issues new access token
7. POST /api/auth/logout revokes the current session
```

## AuthTokenDto

| Field | Description |
|-------|-------------|
| `accessToken` | JWT for Authorization header |
| `expiresAtUtc` | Access token expiry |
| `refreshToken` | Opaque token for refresh endpoint |
| `refreshTokenExpiresAtUtc` | Session expiry (`Jwt:RefreshTokenExpiryDays`) |

## JWT Claims

The access token carries:

- `sub` — user ID
- `role` — role names
- `permission` — permission codes (e.g. `users.read`)

## Auth Facade (Laravel-style)

Two ways to access the current user in handlers:

1. **Inject `IAuth`** (preferred, testable)
2. **Static `Auth` facade** — `Auth.Id`, `Auth.Check`, `Auth.User()`, `Auth.Can(permission)`

The facade is wired at startup via `app.UseFacades()` and is valid only inside HTTP requests.

## Endpoints

| Method | Path | Auth required |
|--------|------|---------------|
| POST | `/api/auth/login` | No |
| POST | `/api/auth/refresh` | No |
| POST | `/api/auth/logout` | Yes |
| POST | `/api/auth/logout-all` | Yes |
| GET | `/api/auth/sessions` | Yes |

## Related

- [Login](/docs/authentication/login)
- [Refresh Tokens](/docs/authentication/refresh-tokens)
- [RBAC Overview](/docs/authorization/overview)
