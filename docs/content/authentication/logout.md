# Logout

Revoke the current session or all sessions for the authenticated user.

## Logout Current Session

| Method | Path | Permission |
|--------|------|------------|
| POST | `/api/v1/auth/logout` | Authenticated |

### Request Body

```json
{
  "refreshToken": "the-refresh-token-to-revoke"
}
```

### Response (200)

```json
{
  "isSuccess": true,
  "data": {
    "message": "Logged out successfully."
  }
}
```

The session behind the provided refresh token is revoked. The access token may remain valid until expiry but cannot be refreshed.

## Logout All Sessions

| Method | Path | Permission |
|--------|------|------------|
| POST | `/api/v1/auth/logout-all` | Authenticated |

No request body. Revokes **every** active session for the current user.

### Response (200)

```json
{
  "isSuccess": true,
  "data": {
    "message": "All sessions revoked."
  }
}
```

## Implementation

- `LogoutCommand` — revokes session by refresh token via `Auth.Logout()`
- `LogoutAllCommand` — revokes all sessions via `Auth.LogoutAll()`

## Related

- [Sessions](/docs/authentication/sessions)
- [Refresh Tokens](/docs/authentication/refresh-tokens)
