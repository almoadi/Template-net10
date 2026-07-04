# RBAC Overview

Template-net10 implements **Role-Based Access Control (RBAC)** with a permission catalog seeded into the database.

## Model

```
User ──(UserRole)──► Role ──(RolePermission)──► Permission
```

- A **user** can have multiple **roles**.
- A **role** can have multiple **permissions**.
- Permissions are fine-grained capability codes (e.g. `users.read`, `users.write`).
- JWT access tokens carry both `role` and `permission` claims.

## Default Roles

| Role | Description |
|------|-------------|
| Admin | All permissions |
| User | Basic authenticated access |

Seeded by `RoleSeeder` and `UserSeeder`.

## Permission Catalog

| Code | English | Arabic |
|------|---------|--------|
| `users.read` | View users | عرض المستخدمين |
| `users.write` | Manage users | إدارة المستخدمين |
| `roles.read` | View roles | عرض الأدوار |
| `roles.write` | Manage roles | إدارة الأدوار |
| `permissions.read` | View permissions | عرض الصلاحيات |

Defined in `PermissionRegistry` and seeded additively by `PermissionSeeder`.

## Authorization Flow

1. Client sends JWT with `permission` claims.
2. Controller action has `[HasPermission("users.read")]`.
3. `PermissionPolicyProvider` resolves the policy.
4. `PermissionAuthorizationHandler` checks the caller's claims.
5. Missing permission → HTTP 403.

## Adding a New Permission

1. Constant in `AuthPermissionCodes`.
2. Entry in `PermissionRegistry` (code + EN/AR).
3. `[HasPermission(...)]` on endpoint.
4. Re-seed and assign to roles.

## When & How to Use It

Authorization decides *what* a signed-in user is allowed to do. Use it whenever an action should
be restricted to certain people:

- **Protect an endpoint** — put `[HasPermission("users.write")]` on a controller action so only
  users whose role grants that permission can call it. Everyone else gets a 403.
- **Separate admins from regular users** — give the `Admin` role every permission and the `User`
  role only the basics, then assign roles to people.
- **Add a brand-new capability** — when you build a new feature (say, exporting reports), create a
  permission like `reports.export`, guard the endpoint with it, and grant it to the roles that
  should have it.
- **Check access inside code** — call `Auth.Can("roles.write")` in a handler when a single
  endpoint behaves differently depending on the caller's permissions.
- **Gate a whole area by role** — use `[HasRole("Admin")]` when you want a coarse, role-level
  check instead of a fine-grained permission.

**Rule of thumb:** prefer fine-grained *permissions* on endpoints, and use *roles* as bundles of
permissions you assign to people.

## Related

- [Permissions](/docs/authorization/permissions)
- [HasPermission](/docs/authorization/has-permission)
- [Roles](/docs/authorization/roles)
