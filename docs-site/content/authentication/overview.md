# Authentication Overview

Template-net10 uses **JWT access tokens** combined with **server-side refresh-token sessions**. Login is by **email + password**.

## Authentication Flow

```
1. Client POST /api/v1/auth/login (email + password)
2. Server validates credentials, creates UserSession row (hashed refresh token)
3. Server returns AuthTokenDto:
   - accessToken (JWT, short-lived)
   - refreshToken (opaque, long-lived)
4. Client sends accessToken as Bearer token on protected requests
5. When access token expires, POST /api/v1/auth/refresh with refreshToken
6. Server rotates refresh token, issues new access token
7. POST /api/v1/auth/logout revokes the current session
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
| POST | `/api/v1/auth/login` | No |
| POST | `/api/v1/auth/refresh` | No |
| POST | `/api/v1/auth/logout` | Yes |
| POST | `/api/v1/auth/logout-all` | Yes |
| GET | `/api/v1/auth/sessions` | Yes |

## When & How to Use It

Authentication is the front door of your app — reach for it whenever you need to know *who* is
making a request. In practice you'll use it to:

- **Sign a user in** — a login screen collects email + password and calls `POST /api/v1/auth/login`.
  You store the returned `accessToken` and `refreshToken` on the client.
- **Keep a user signed in** — when the short-lived access token expires, silently call
  `POST /api/v1/auth/refresh` with the refresh token instead of forcing them to log in again.
- **Log out one device** — call `POST /api/v1/auth/logout` to revoke just the current session
  (for example, a "Sign out" button).
- **Log out everywhere** — call `POST /api/v1/auth/logout-all` after a password change or a
  "suspicious activity" alert to kill every session for that user.
- **Show active devices** — call `GET /api/v1/auth/sessions` to power a "Where you're logged in"
  screen so users can review and revoke sessions.
- **Read the current user in code** — inject `IAuth` (or use the static `Auth` facade) inside a
  handler to get the caller's id, roles, and permissions.

**Typical flow:** log in once → send the access token on every request → refresh quietly in the
background → log out when done.

## Related

- [Login](/docs/authentication/login)
- [Refresh Tokens](/docs/authentication/refresh-tokens)
- [RBAC Overview](/docs/authorization/overview)
