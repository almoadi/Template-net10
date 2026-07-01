# Configuration Overview

Settings are organized in Laravel-style JSON files under `src/API/config/`, loaded at startup and bound to strongly-typed options classes.

## Loading Mechanism

```csharp
foreach (var file in new[] { "app", "database", "cache", "mail", "jwt", "queue", "cors" })
{
    builder.Configuration
        .AddJsonFile($"config/{file}.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"config/{builder.Environment.EnvironmentName}/{file}.json", optional: true, reloadOnChange: true);
}
```

## File Structure

```
src/API/config/
├── app.json
├── database.json
├── cache.json
├── mail.json
├── jwt.json
├── queue.json
├── cors.json
├── Development/          # per-environment overrides
├── Staging/
└── Production/
```

## Options Classes

Each config file maps to a class in `Infrastructure/Options/`:

| Config | Options class |
|--------|---------------|
| `app.json` | `AppOptions` |
| `database.json` | `DatabaseOptions` |
| `cache.json` | `CacheOptions` |
| `mail.json` | `MailOptions` |
| `jwt.json` | `JwtOptions` |
| `queue.json` | `QueueOptions` |

Registered in `Infrastructure/DependencyInjection.cs` via `services.Configure<T>(...)`.

## Environment Overrides

Base values apply to all environments. Files in `config/{Environment}/` override specific keys:

```
config/mail.json              → Driver: Log
config/Production/mail.json   → Driver: Smtp, real Host/Port
```

## Secrets

Never commit production secrets. Override via:

- `dotnet user-secrets`
- Environment variables (`Jwt__SecretKey`)
- Azure Key Vault / similar

## Related

- [Getting Started: Configuration](/docs/getting-started/configuration)
- [JWT](/docs/configuration/jwt)
