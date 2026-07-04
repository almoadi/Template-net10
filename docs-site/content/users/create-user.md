# Create User

Create a new user account with optional role assignments.

## Endpoint

| Method | Path | Permission |
|--------|------|------------|
| POST | `/api/v1/auth/users` | `users.write` |

## Request Body

```json
{
  "nameEn": "Jane Doe",
  "nameAr": "جane Doe",
  "email": "jane@example.com",
  "phone": "+966500000000",
  "password": "SecureP@ss123",
  "roleIds": [2]
}
```

## Success Response (200)

Returns the new user's ID:

```json
{
  "isSuccess": true,
  "data": 42
}
```

## Validation

- Email must be unique and valid format
- Password must meet complexity requirements (see `CreateUserValidator`)
- Role IDs must reference existing roles

## Example

```http
POST /api/v1/auth/users
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

{
  "nameEn": "Jane Doe",
  "nameAr": "Jane Doe",
  "email": "jane@example.com",
  "phone": "",
  "password": "SecureP@ss123",
  "roleIds": [2]
}
```

## Related

- [Users Overview](/docs/users/overview)
- [Assign Roles](/docs/users/assign-roles)
