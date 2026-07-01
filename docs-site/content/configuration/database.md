# Database Configuration

Entity Framework Core connection and behaviour settings.

**File:** `src/API/config/database.json`  
**Options class:** `DatabaseOptions`

## Settings

| Key | Default | Description |
|-----|---------|-------------|
| `Database:ConnectionString` | (see file) | SQL Server connection string |
| `Database:EnableDetailedErrors` | `false` | Verbose EF errors — dev only |
| `Database:EnableSensitiveDataLogging` | `false` | Log parameter values — dev only |
| `Database:CommandTimeoutSeconds` | `30` | SQL command timeout |
| `Database:MaxRetryCount` | `3` | Transient fault retry attempts |

## Connection String Fallback

If `Database:ConnectionString` is blank, the app falls back to `ConnectionStrings:DefaultConnection`.

## Override via Environment

```powershell
dotnet user-secrets set "Database:ConnectionString" "Server=...;Database=MyApp;..."
```

## Provider

SQL Server via `Microsoft.EntityFrameworkCore.SqlServer`. Hangfire also uses the same database for job storage.

## Related

- [Database Overview](/docs/database/overview)
- [Migrations](/docs/database/migrations)
