# Permissions

The permission catalog defines every capability the application knows about. Permissions are seeded from code and exposed via a read-only API.

## List Permissions

| Method | Path | Permission |
|--------|------|------------|
| GET | `/api/v1/auth/permissions` | `permissions.read` |

### Response (200)

```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "code": "users.read",
      "nameEn": "View users",
      "nameAr": "عرض المستخدمين"
    }
  ]
}
```

## Permission Codes

Constants in `AuthPermissionCodes`:

| Constant | Code |
|----------|------|
| `UsersRead` | `users.read` |
| `UsersWrite` | `users.write` |
| `RolesRead` | `roles.read` |
| `RolesWrite` | `roles.write` |
| `PermissionsRead` | `permissions.read` |

## Catalog Source

`PermissionRegistry.All` is the single source of truth. The seeder reads this list to create/update rows in the `Permissions` table — adding a new capability is a one-line change plus re-seed.

## JWT Claims

When a user logs in, their effective permissions (union of all assigned roles) are embedded as `permission` claims in the access token.

## Related

- [RBAC Overview](/docs/authorization/overview)
- [HasPermission](/docs/authorization/has-permission)
- [Assign Roles](/docs/users/assign-roles)
