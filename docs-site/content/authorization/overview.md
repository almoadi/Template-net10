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

## Related

- [Permissions](/docs/authorization/permissions)
- [HasPermission](/docs/authorization/has-permission)
- [Roles](/docs/authorization/roles)
