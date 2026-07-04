# Sessions

List active login sessions for the currently authenticated user.

## Endpoint

| Method | Path | Permission |
|--------|------|------------|
| GET | `/api/v1/auth/sessions` | Authenticated (any user) |

## Response (200)

```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "deviceInfo": "Mozilla/5.0 ...",
      "ipAddress": "127.0.0.1",
      "createdAtUtc": "2026-07-01T10:00:00Z",
      "expiresAtUtc": "2026-07-31T10:00:00Z",
      "isCurrent": true
    }
  ]
}
```

## UserSession Entity

Each login creates a `UserSession` row storing:

- Hashed refresh token (never stored in plain text)
- Expiry timestamp
- Device info / user agent
- IP address
- Revocation status

Sessions are created by `Auth.Attempt()` during login and revoked by logout endpoints.

## Example

```http
GET /api/v1/auth/sessions
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

## Related

- [Logout](/docs/authentication/logout)
- [Authentication Overview](/docs/authentication/overview)
