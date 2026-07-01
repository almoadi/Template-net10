# Configuration

Template-net10 uses a **Laravel-style** configuration system. Settings live in `src/API/config/`, one JSON file per concern, each bound to a strongly-typed options class in `src/Infrastructure/Options/`.

## How Config Is Loaded

In `Program.cs`, each concern is loaded from a base file plus an optional per-environment override:

```csharp
foreach (var file in new[] { "app", "database", "cache", "mail", "jwt", "queue", "cors" })
{
    builder.Configuration
        .AddJsonFile($"config/{file}.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"config/{builder.Environment.EnvironmentName}/{file}.json", optional: true, reloadOnChange: true);
}
```

| Layer | Path | Purpose |
|-------|------|---------|
| Base | `config/{name}.json` | Default values for all environments |
| Override | `config/{Environment}/{name}.json` | Development, Staging, Production overrides |

Files support `//` comments and trailing commas (the .NET JSON config provider allows both).

## Config Files

| File | Options class | Documentation |
|------|---------------|---------------|
| `app.json` | `AppOptions` | [App config](/docs/configuration/app) |
| `database.json` | `DatabaseOptions` | [Database config](/docs/configuration/database) |
| `cache.json` | `CacheOptions` | [Cache config](/docs/configuration/cache) |
| `mail.json` | `MailOptions` | [Mail config](/docs/configuration/mail) |
| `jwt.json` | `JwtOptions` | [JWT config](/docs/configuration/jwt) |
| `queue.json` | `QueueOptions` | [Queue config](/docs/configuration/queue) |
| `cors.json` | — | [CORS config](/docs/configuration/cors) |

## Secrets

> **Important:** Never commit production secrets. The following must come from user-secrets, environment variables, or a vault:

- `Jwt:SecretKey`
- Database connection strings
- SMTP credentials
- Redis connection strings

Use `dotnet user-secrets` for local development:

```powershell
cd src/API
dotnet user-secrets set "Jwt:SecretKey" "YOUR_64_CHAR_SECRET_HERE"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=..."
```

Environment variables use double-underscore nesting: `Jwt__SecretKey`, `ConnectionStrings__DefaultConnection`.

## Related

- [Configuration Overview](/docs/configuration/overview) — detailed reference for every section
- [JWT](/docs/configuration/jwt) — token signing and expiry settings
