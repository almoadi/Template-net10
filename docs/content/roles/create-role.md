# Create Role

Create a new role with optional permission assignments.

## Endpoint

| Method | Path | Permission |
|--------|------|------------|
| POST | `/api/v1/auth/roles` | `roles.write` |

## Request Body

```json
{
  "nameEn": "Editor",
  "nameAr": "محرر",
  "permissionIds": [1, 2]
}
```

## Success Response (200)

```json
{
  "isSuccess": true,
  "data": 5
}
```

Returns the new role's ID.

## Example

```http
POST /api/v1/auth/roles
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

{
  "nameEn": "Editor",
  "nameAr": "محرر",
  "permissionIds": [1, 2, 3]
}
```

## Related

- [Search Roles](/docs/roles/search-roles)
- [Permissions](/docs/authorization/permissions)
