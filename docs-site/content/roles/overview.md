# Roles API Overview

The Roles API manages role definitions and their permission assignments.

## Endpoints Summary

| Method | Path | Permission |
|--------|------|------------|
| GET | `/api/v1/auth/roles` | `roles.read` |
| POST | `/api/v1/auth/roles` | `roles.write` |

## RoleDto Shape

| Field | Type | Description |
|-------|------|-------------|
| `id` | int | Primary key |
| `nameEn` | string | English name |
| `nameAr` | string | Arabic name |
| `permissions` | string[] | Permission codes assigned to this role |

## Controller

`RolesController` at route `api/v1/auth/roles`.

## When & How to Use It

Roles are reusable bundles of permissions you hand out to users. Use the Roles API when you're
building the admin side of access control:

- **Create a new role** — `POST /api/v1/auth/roles` when your app needs a new tier of access, such
  as `Editor` or `Support`, with a specific set of permissions.
- **Populate an "assign role" dropdown** — call `GET /api/v1/auth/roles` to list roles when an admin
  is choosing what to grant a user (see [Assign Roles](/docs/users/assign-roles)).
- **Review who can do what** — each role returns its `permissions[]`, so you can show exactly
  which capabilities a role unlocks.
- **Grow permissions over time** — when you add a new permission to the catalog, update the roles
  that should include it rather than editing every user.

**Typical flow:** define permissions → group them into a role → assign the role to users. Change
the role once and everyone with it is updated.

## Related

- [Create Role](/docs/roles/create-role)
- [Search Roles](/docs/roles/search-roles)
- [Roles (Authorization)](/docs/authorization/roles)
