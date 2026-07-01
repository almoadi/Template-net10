# Roles API Overview

The Roles API manages role definitions and their permission assignments.

## Endpoints Summary

| Method | Path | Permission |
|--------|------|------------|
| GET | `/api/auth/roles` | `roles.read` |
| POST | `/api/auth/roles` | `roles.write` |

## RoleDto Shape

| Field | Type | Description |
|-------|------|-------------|
| `id` | int | Primary key |
| `nameEn` | string | English name |
| `nameAr` | string | Arabic name |
| `permissions` | string[] | Permission codes assigned to this role |

## Controller

`RolesController` at route `api/auth/roles`.

## Related

- [Create Role](/docs/roles/create-role)
- [Search Roles](/docs/roles/search-roles)
- [Roles (Authorization)](/docs/authorization/roles)
