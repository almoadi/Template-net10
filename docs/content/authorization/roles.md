# Roles (Authorization)

Roles group permissions and are assigned to users. This page covers the authorization model; see [Roles API](/docs/roles/overview) for HTTP endpoints.

## Role Entity

A role has:

- Name (English and Arabic)
- A set of permissions via `RolePermission` join table
- Many users via `UserRole` join table

## Default Seeded Roles

| Role | Permissions |
|------|-------------|
| Admin | All permissions in the catalog |
| User | None by default (customize in `RoleSeeder`) |

## Assigning Permissions to Roles

When creating a role via `POST /api/v1/auth/roles`, pass permission IDs. The `CreateRoleCommand` links permissions through the domain entity.

Users inherit permissions from all their roles. The JWT contains the union of permissions.

## Checking Permissions in Code

```csharp
// In a handler via IAuth
if (!await _auth.Can(AuthPermissionCodes.UsersWrite, ct))
    throw new ForbiddenAccessException(...);

// Via static facade (HTTP context only)
if (!Auth.Can(AuthPermissionCodes.UsersWrite)) { ... }
```

Prefer `[HasPermission]` on controllers for endpoint-level authorization.

## Related

- [Roles API](/docs/roles/overview)
- [Assign Roles](/docs/users/assign-roles)
- [Permissions](/docs/authorization/permissions)
