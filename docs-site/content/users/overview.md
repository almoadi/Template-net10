# Users Overview

The Users API manages user accounts — create, search, retrieve, and assign roles. All endpoints require appropriate permissions.

## Endpoints Summary

| Method | Path | Permission |
|--------|------|------------|
| POST | `/api/v1/auth/users` | `users.write` |
| GET | `/api/v1/auth/users` | `users.read` |
| GET | `/api/v1/auth/users/{id}` | `users.read` |
| PUT | `/api/v1/auth/users/{id}/roles` | `users.write` |

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

`UsersController` at route `api/v1/auth/users` — all actions delegate to MediatR.

## When & How to Use It

The Users API is what you wire up behind an admin "User management" screen. Use it to:

- **Create an account** — `POST /api/v1/auth/users` when an admin onboards a new team member or
  customer. You supply names, email, phone, and a password.
- **Build a searchable user list** — `GET /api/v1/auth/users` powers a paged, searchable table so
  admins can find people by name or email.
- **Show a user's profile** — `GET /api/v1/auth/users/{id}` when opening a single user's detail page.
- **Change what a user can do** — `PUT /api/v1/auth/users/{id}/roles` to grant or revoke access by
  swapping their roles (no need to touch individual permissions).

**Typical flow:** create the user → find them in the list → open their profile → assign the roles
that match their job. Deactivating (`isActive = false`) or soft-deleting keeps history intact
instead of hard-deleting the record.

## Related

- [Create User](/docs/users/create-user)
- [Search Users](/docs/users/search-users)
- [Assign Roles](/docs/users/assign-roles)
