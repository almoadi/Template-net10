# Seeders

Database seeding follows a Laravel-style idempotent pattern. Seeders run automatically after migrations on startup.

## Seeder Chain

```
DatabaseSeeder
├── PermissionSeeder    (additive — adds new permissions without deleting)
├── RoleSeeder          (Admin + User roles)
└── UserSeeder          (default admin user)
```

Located in `Infrastructure/Seeders/`.

## Default Admin User

| Field | Value |
|-------|-------|
| Email | `admin@template-net10.local` |
| Password | `ChangeMe!123` |
| Role | Admin (all permissions) |

> Change these credentials before deploying to any shared environment.

## Permission Seeder

Reads from `PermissionRegistry.All`. Adding a new permission to the registry and re-running the app adds it to the database without removing existing rows.

## Base Seeder Class

All seeders inherit from `Seeder` which provides logging and idempotent helpers.

## Manual Re-Seed

Seeders run on every startup via `DatabaseSeederExtensions`. To force re-seed in development, restart the API after changing seeder code.

## Related

- [RBAC Overview](/docs/authorization/overview)
- [Permissions](/docs/authorization/permissions)
