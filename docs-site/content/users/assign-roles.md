# Assign Roles

Replace the roles assigned to a user.

## Endpoint

| Method | Path | Permission |
|--------|------|------------|
| PUT | `/api/v1/auth/users/{id}/roles` | `users.write` |

## Request Body

```json
{
  "roleIds": [1, 2]
}
```

The route `{id}` is always applied to `UserId` — the body cannot override it.

## Response (200)

```json
{
  "isSuccess": true,
  "data": {
    "message": "Roles assigned successfully."
  }
}
```

## Behaviour

- Replaces the user's entire role set with the provided IDs.
- User must exist; role IDs must be valid.
- After assignment, the user must **log in again** (or refresh token) to receive updated JWT claims.

## Example

```http
PUT /api/v1/auth/users/42/roles
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

{
  "roleIds": [1]
}
```

## Related

- [Roles (Authorization)](/docs/authorization/roles)
- [Create User](/docs/users/create-user)
