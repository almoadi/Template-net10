# Users Overview

The Users API manages user accounts — create, search, retrieve, and assign roles. All endpoints require appropriate permissions.

## Endpoints Summary

| Method | Path | Permission |
|--------|------|------------|
| POST | `/api/auth/users` | `users.write` |
| GET | `/api/auth/users` | `users.read` |
| GET | `/api/auth/users/{id}` | `users.read` |
| PUT | `/api/auth/users/{id}/roles` | `users.write` |

## UserDto Shape

| Field | Type | Description |
|-------|------|-------------|
| `id` | int | Primary key |
| `nameEn` | string | English display name |
| `nameAr` | string | Arabic display name |
| `email` | string | Login email (unique) |
| `phone` | string | Phone number |
| `isActive` | bool | Account active flag |
| `roles` | string[] | Assigned role names |

## Controller

`UsersController` at route `api/auth/users` — all actions delegate to MediatR.

## Related

- [Create User](/docs/users/create-user)
- [Search Users](/docs/users/search-users)
- [Assign Roles](/docs/users/assign-roles)
