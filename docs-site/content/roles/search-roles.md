# Search Roles

List roles with pagination.

## Endpoint

| Method | Path | Permission |
|--------|------|------------|
| GET | `/api/v1/auth/roles` | `roles.read` |

## Query Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `offset` | int | Records to skip (default 0) |
| `limit` | int | Page size (default 20, max 100) |
| `search` | string | Filter by role name |

## Response (200)

```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "nameEn": "Admin",
      "nameAr": "مدير",
      "permissions": ["users.read", "users.write", "roles.read", "roles.write", "permissions.read"]
    }
  ],
  "metaData": {
    "resultSet": { "count": 2, "limit": 20, "offset": 0 }
  }
}
```

## Example

```http
GET /api/v1/auth/roles?offset=0&limit=20
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

## Related

- [Create Role](/docs/roles/create-role)
- [Pagination](/docs/api/pagination)
