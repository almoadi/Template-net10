# Configuration

Template-net10 uses a **Laravel-style** configuration system. Settings live in `src/API/config/`, one JSON file per concern, each bound to a strongly-typed options class in `src/Infrastructure/Options/`.

## How Config Is Loaded

In `Program.cs`, configuration is composed through a single extension:

```csharp
builder.AddSplitConfiguration();
```

Defined in `src/API/Extensions/ConfigurationExtensions.cs`, it loads each concern from a base file
plus an optional per-environment override, then applies environment variables last:

```csharp
foreach (var file in ConfigFiles)
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
| `storage.json` | `StorageOptions` | [Storage config](/docs/configuration/storage) |
| `auth.json` | `AuthOptions` | [Auth config](/docs/configuration/auth) |
| `socialite.json` | `SocialiteOptions` | [Socialite config](/docs/configuration/socialite) |
| `features.json` | `FeatureFlagsOptions` | [Feature Flags config](/docs/configuration/features) |
| `encryption.json` | `EncryptionOptions` | [Encryption config](/docs/configuration/encryption) |
| `idempotency.json` | `IdempotencyOptions` | [Idempotency config](/docs/configuration/idempotency) |

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
