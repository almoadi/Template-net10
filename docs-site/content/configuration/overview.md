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

## When & How to Use It

Configuration is how you change behavior without recompiling. Reach for it when:

- **A value differs per environment** — database connection strings, the mail driver, or the cache
  driver that should be `Log`/`Memory` locally but `Smtp`/`Redis` in production. Put the base value
  in `config/{name}.json` and override just the changed keys in `config/{Environment}/{name}.json`.
- **You're adding a new setting** — create the JSON file, add its name to the loader loop in
  `Program.cs`, and bind it to a strongly-typed `{Name}Options` class so handlers read typed values,
  not magic strings.
- **A value is a secret** — keep API keys, the JWT secret, and real connection strings out of the
  repo. Supply them through `dotnet user-secrets`, environment variables, or a vault instead.
- **You want a live tweak** — files are loaded with `reloadOnChange`, so many settings take effect
  without a restart.

**Rule of thumb:** one file per concern, typed options over raw strings, and secrets never committed.

## Related

- [Getting Started: Configuration](/docs/getting-started/configuration)
- [JWT](/docs/configuration/jwt)
