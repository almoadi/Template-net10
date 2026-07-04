# Search Users

Search and list users with pagination.

## Endpoint

| Method | Path | Permission |
|--------|------|------------|
| GET | `/api/v1/auth/users` | `users.read` |

## Query Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `offset` | int | Records to skip (default 0) |
| `limit` | int | Page size (default 20, max 100) |
| `search` | string | Filter by name or email |

## Response (200)

```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "nameEn": "Admin",
      "nameAr": "مدير",
      "email": "admin@template-net10.local",
      "phone": "",
      "isActive": true,
      "roles": ["Admin"]
    }
  ],
  "metaData": {
    "resultSet": { "count": 1, "limit": 20, "offset": 0 }
  }
}
```

## Example

```http
GET /api/v1/auth/users?offset=0&limit=20&search=admin
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

## Related

- [Pagination](/docs/api/pagination)
- [Get User](/docs/users/get-user)
