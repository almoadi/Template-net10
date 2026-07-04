# Pagination

Paged list endpoints accept query parameters via `PagedRequest` and return `PagedApiResponseDto<T>`.

## Query Parameters

| Parameter | Default | Max | Description |
|-----------|---------|-----|-------------|
| `offset` | `0` | — | Number of records to skip |
| `limit` | `20` | `100` | Page size |

Example:

```http
GET /api/v1/auth/users?offset=0&limit=20&search=admin
```

## Handler Pattern

Query handlers filter, order, project, then call the extension method:

```csharp
return await _context.Users
    .AsNoTracking()
    .Where(...)
    .OrderBy(x => x.Id)
    .Select(x => new UserDto { ... })
    .ToPagedResponseAsync(query, ct);
```

All counting and page-bound clamping live in `PaginationExtensions` and `PagedApiResponseFactory` — change paging behaviour in one place.

## Response Shape

```json
{
  "isSuccess": true,
  "data": [ ... ],
  "metaData": {
    "resultSet": {
      "count": 137,
      "limit": 20,
      "offset": 0
    }
  }
}
```

- `count` — total matching records (not just the current page)
- `limit` — applied page size (clamped to max 100)
- `offset` — applied skip value

## Endpoints Using Pagination

| Endpoint | Query class |
|----------|-------------|
| `GET /api/v1/auth/users` | `SearchUsersQuery` |
| `GET /api/v1/auth/roles` | `SearchRolesQuery` |

## Related

- [Response Envelope](/docs/api/response-envelope)
- [Search Users](/docs/users/search-users)
