# Get User

Retrieve a single user by ID.

## Endpoint

| Method | Path | Permission |
|--------|------|------------|
| GET | `/api/v1/auth/users/{id}` | `users.read` |

## Response (200)

```json
{
  "isSuccess": true,
  "data": {
    "id": 1,
    "nameEn": "Admin",
    "nameAr": "مدير",
    "email": "admin@template-net10.local",
    "phone": "",
    "isActive": true,
    "roles": ["Admin"]
  }
}
```

## Not Found

Returns `isSuccess: false` with a localized message when the user does not exist.

## Example

```http
GET /api/v1/auth/users/1
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

## Related

- [Search Users](/docs/users/search-users)
- [Create User](/docs/users/create-user)
