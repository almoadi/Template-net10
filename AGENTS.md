# AGENTS.md

Operating guide for AI coding agents working in this repository. Read this first, then consult
[`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) for the deep reference. **If guidance here conflicts
with older code comments, this file and `docs/ARCHITECTURE.md` win.**

---

## What this is

**Template-net10** is a production-ready **.NET 10** backend **starter kit** — the foundation copied
into new projects. It is a monolith built on **Clean Architecture + CQRS + MediatR**, wired into
**.NET Aspire**. Many conventions deliberately mirror **Laravel** (config folder, seeders, the `Auth`
facade, YAML lang files).

Because it is a starter kit, favor **clarity, consistency, and completeness** over cleverness. New
code must look like the existing code.

---

## Solution map

```
src/Domain          # entities, value objects, enums, constants, domain exceptions — depends on NOTHING
src/Application     # CQRS use cases, abstractions, behaviours, DTOs — depends on Domain only
src/Infrastructure  # EF Core, services, auth, seeders, jobs — implements Application/Domain abstractions
src/API             # ASP.NET host, controllers, startup wiring, config/ + resources/
Tests/Template-net10.UnitTests   # NUnit
Template-net10.AppHost           # Aspire orchestrator (runs the API)
Template-net10.ServiceDefaults   # Aspire telemetry/health/resilience
```

Dependencies point **inward**: `API → Application → Domain`, and `Infrastructure → Application/Domain`.
Domain references no framework packages. `IApplicationDbContext` lives in
`Application/Abstractions/Data` (not Domain) because it exposes EF's `DbSet<T>`.

### Naming
- **Namespaces / assembly names** use an underscore: `Template_net10.Application`, etc.
- **Project / folder names** use a hyphen: `Template-net10.Application.csproj`.
- **Namespace must equal folder path.**

---

## Build, test, run (Windows / PowerShell)

```powershell
dotnet build Template-net10.slnx
dotnet test  Tests/Template-net10.UnitTests/Template-net10.UnitTests.csproj
dotnet run   --project Template-net10.AppHost     # run via Aspire (preferred)
dotnet run   --project src/API                    # run the API directly
```

**A task is not complete until `dotnet build` is green (0 errors) and tests pass.**

### Gotcha: locked DLLs while the API is running
If the API is running (`dotnet run`), a full-solution build fails with `MSB3027`/`MSB3021` "file is
locked" on `src/API/bin/...`. This is **not a compile error**. Either stop the running app, or verify
compilation another way (`dotnet build src/API/...csproj -o <tempdir>`), and rely on `dotnet test`
(which builds Domain/Application/Infrastructure + tests, not the locked API bin).

---

## Configuration (Laravel-style `config/`)

Settings live in `src/API/config/`, one file per concern, each bound to a strongly-typed options class
in `src/Infrastructure/Options/`. Loaded in [`src/API/Program.cs`](src/API/Program.cs):

```csharp
foreach (var file in new[] { "app", "database", "cache", "mail", "jwt", "queue" })
{
    builder.Configuration
        .AddJsonFile($"config/{file}.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"config/{builder.Environment.EnvironmentName}/{file}.json", optional: true, reloadOnChange: true);
}
```

- **Base** files: `config/{name}.json`. **Per-environment overrides:** `config/{Environment}/{name}.json`
  (e.g. `config/Development/queue.json`, `config/Production/mail.json`).
- Files carry `//` comments documenting **available driver values** — the .NET JSON config provider
  allows comments and trailing commas. Keep those comments accurate when you add options.
- `config/**/*.json` is copied to output via a `<Content Update=...>` item in the API csproj.
- Drivers: `cache` → `Memory`|`Redis`; `mail` → `Log`|`Smtp`; `queue` → `Hangfire`.

> Secrets (`jwt.json` SecretKey, `mail.json` credentials, real DB connection strings) must come from
> user-secrets / environment variables / a vault — never committed.

---

## Key features & where they live

| Feature | Entry points |
|---------|-------------|
| **RBAC** | `Domain/Auth/Entities` (`User`,`Role`,`Permission`,`UserRole`,`RolePermission`); codes in `Domain/Auth/Constants` |
| **Auth (Laravel facade)** | `IAuth` + static `Auth` (`Application`), `AuthService` (`Infrastructure/Services`) — login is **email + password** |
| **Authorization** | `[HasPermission(AuthPermissionCodes.X)]` → `PermissionPolicyProvider` + `PermissionAuthorizationHandler` |
| **Localization** | YAML files `src/API/resources/lang/{en,ar}.yml`; `LocalizationService` resolves by request culture |
| **Mail** | `IEmailSender` (`SendAsync`/`SendBulkAsync`) → `SmtpEmailSender` (MailKit); no HTTP endpoint — call from your own code |
| **Queue / jobs** | `IJobScheduler` (`Application`) → Hangfire (`Infrastructure`); jobs in `Application/Abstractions/Jobs`; dashboard at `/hangfire` |
| **Seeders** | Laravel-style under `Infrastructure/Seeders` (`DatabaseSeeder` → Permission → Role → User) |
| **Response envelope** | `ApiResponseDto<T>`, `PagedApiResponseDto<T>`, `MessageDto` |

Default seeded admin (dev): `admin@template-net10.local` / `ChangeMe!123`.

---

## How to extend (follow these recipes)

**Add a use case (endpoint action).** Create a self-contained folder
`Application/{Area}/{Feature}/Commands|Queries/{Action}/` with `{Action}Command|Query.cs`,
`{Action}Handler.cs`, and (for commands, or queries needing input validation) `{Action}Validator.cs`.
Add a controller action that inherits `ApiControllerBase`, declares `[HasPermission]` +
`[ProducesResponseType]`, and **only** calls `Sender.Send(...)`.

**Writes:** command handler loads/tracks the entity via `IApplicationDbContext` and calls
`SaveChangesAsync`. **Reads:** query handler uses `AsNoTracking()` + `.Select(...)` projection; never
`SaveChangesAsync`. Paged reads end with `.ToPagedResponseAsync(query, ct)`.

**Business logic goes in the Domain entity** (private ctor/setters, static `Create`, instance methods),
never in handlers.

**Add an entity:** create it in `Domain`, add an `IEntityTypeConfiguration<T>` in
`Infrastructure/Configurations`, expose `DbSet<T>` on **both** `IApplicationDbContext` and
`ApplicationDbContext`, then generate a migration (below).

**Add a permission:** constant in `AuthPermissionCodes` → catalog entry in `PermissionRegistry`
(code + EN/AR) → decorate endpoint → re-seed (PermissionSeeder is additive) → assign to a role.

**Add a user-facing message:** add a key to the `Resource` enum, then add a matching line to **every**
`resources/lang/*.yml` file (`en.yml`, `ar.yml`).

**Add a config section:** create `config/{name}.json` (+ optional per-env overrides), add `"{name}"` to
the loop in `Program.cs`, add a `{Name}Options` class in `Infrastructure/Options`, and
`services.Configure<{Name}Options>(...)` in `AddInfrastructure`.

**Add a background job:** define an interface in `Application/Abstractions/Jobs`, implement it in
`Infrastructure/Services` (auto-registered by Scrutor), enqueue via
`_jobScheduler.Enqueue<IMyJob>(j => j.RunAsync(args))`.

### EF migrations (CLI only — never hand-author)
```powershell
dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/API --output-dir Data/Migrations
dotnet ef database update        --project src/Infrastructure --startup-project src/API
```

---

## Packages
**Central Package Management** is on: declare versions in
[`Directory.Packages.props`](Directory.Packages.props); `<PackageReference>` entries in csproj files
carry **no `Version`**. Use the latest stable version, and put each reference in the layer that needs it.

---

## Golden rules (do not break)

1. No Repository pattern — handlers use `IApplicationDbContext` directly.
2. No business logic in handlers — it lives in Domain entities.
3. Query handlers are read-only (`AsNoTracking()` + `.Select(...)`).
4. No business services in controllers — only `Sender.Send(...)`.
5. Entities are externally immutable (private ctor/setters, `Create`/`Update`).
6. No hardcoded user-facing text — use `ILocalizationService` + `Resource` + the YAML files.
7. Namespace == folder path; one use case per folder; never mix command and query files.
8. Domain depends on nothing; Infrastructure implements Application/Domain abstractions.
9. Every runtime DB object ships in a CLI-generated EF migration.
10. Secrets never committed; config is per-concern files with per-environment overrides.
11. Build + tests must be green before a task is done.
