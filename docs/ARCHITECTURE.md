# Template-net10 — Architecture & Starter-Kit Guide

A production-ready **.NET 10** backend starter kit built on **Clean Architecture + CQRS + MediatR**,
wired into **.NET Aspire**. This document explains how the solution is organized, the conventions to
follow, and how to extend it for new projects.

> **TL;DR for a new feature:** add a self-contained folder under
> `Application/{Area}/{Feature}/Commands|Queries/{Action}/`, add a controller action that only calls
> `Sender.Send(...)`, and (for writes) keep the business logic inside the Domain entity.

---

## 1. Technology Stack

| Concern | Technology / Package |
|---------|----------------------|
| Runtime / SDK | .NET 10 |
| Web framework | ASP.NET Core (`Microsoft.NET.Sdk.Web`) |
| Orchestration | .NET Aspire (`AppHost` + `ServiceDefaults`) |
| CQRS / Mediator | `MediatR` |
| Validation | `FluentValidation` (+ DI extensions) |
| ORM (reads + writes) | `Microsoft.EntityFrameworkCore` + `*.SqlServer` + `*.Design` |
| Auth | `Microsoft.AspNetCore.Authentication.JwtBearer`, `System.IdentityModel.Tokens.Jwt` |
| Password hashing | `Microsoft.AspNetCore.Identity.PasswordHasher` (shared framework) |
| Caching | `Microsoft.Extensions.Caching.Memory`, `*.StackExchangeRedis` |
| Mail (SMTP) | `MailKit` |
| Background jobs / queue | `Hangfire` (SQL Server storage) |
| Localization | YAML language files (`YamlDotNet`) |
| DI scanning | `Scrutor` |
| API docs | `Swashbuckle.AspNetCore`, `Microsoft.AspNetCore.OpenApi` |
| Health checks | `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore` |
| Tests | `NUnit`, `Moq`, `FluentAssertions`, `EFCore.InMemory`, `coverlet` |

**Central Package Management** is enabled: every version lives in
[`Directory.Packages.props`](../Directory.Packages.props) and project files reference packages with
`<PackageReference Include="..." />` and **no `Version` attribute**.

Common project properties: `net10.0`, `Nullable` enabled, `ImplicitUsings` enabled.

---

## 2. Solution Layout

```
Template-net10/
├── Template-net10.slnx
├── Directory.Packages.props              # central package versions
├── docs/
│   └── ARCHITECTURE.md                   # this file
├── src/
│   ├── Domain/                           # entities, enums, constants, domain exceptions
│   ├── Application/                       # use cases: commands/queries/handlers/validators, abstractions, behaviours
│   ├── Infrastructure/                   # EF Core, configurations, migrations, services, auth, seeders
│   └── API/                              # host, controllers, startup wiring
├── Tests/
│   └── Template-net10.UnitTests/         # NUnit tests
├── tools/Do/                             # `do` CLI — project tooling (rename, key:generate)
├── Template-net10.AppHost/               # Aspire orchestrator (runs the API)
└── Template-net10.ServiceDefaults/       # Aspire shared telemetry/health/resilience
```

### Dependency rule (points inward)

```
API  →  Application  →  Domain
 │                        ▲
 └────  Infrastructure  ──┘   (implements Application/Domain abstractions)
```

- **Domain** depends on **nothing** (no EF, no MediatR, no ASP.NET).
- **Application** depends only on Domain.
- **Infrastructure** implements Application/Domain abstractions (dependency inversion).
- **API** depends on Application + Infrastructure and composes everything in `Program.cs`.

> **Note on `IApplicationDbContext`:** because it exposes `DbSet<T>` (an EF type) it lives in
> [`Application/Abstractions/Data`](../src/Application/Abstractions/Data/IApplicationDbContext.cs),
> not in Domain — this keeps Domain free of any EF dependency. The EF `ApplicationDbContext` in
> Infrastructure implements it.

---

## 3. Layer-by-layer

### 3.1 Domain (`src/Domain`)

Pure C#. Entities are **externally immutable**: private constructor, `private set` properties, a
static `Create(...)` factory, and instance behaviour methods (`Update`, `AssignRole`, …). All state
transitions live here — never in handlers.

```
Domain/
├── Common/
│   ├── BaseEntity.cs                     # Id + CreatedAt/UpdatedAt
│   ├── LengthConstants.cs                # shared string lengths (schema + validators stay in sync)
│   └── Exceptions/                       # BadRequest / ItemNotFound / ForbiddenAccess / TooManyRequests
└── Auth/
    ├── Entities/                         # User, Role, Permission, UserRole, RolePermission, UserSession
    └── Constants/                        # AuthPermissionCodes, AuthRoles, PermissionRegistry
```

### 3.2 Application (`src/Application`)

Use-case orchestration. One **self-contained folder per use case**.

```
Application/
├── Abstractions/                         # interfaces grouped by concern
│   ├── Data/IApplicationDbContext.cs
│   ├── Security/ (IAuth, ICurrentUserService, IPasswordHasher, IJwtTokenService)
│   ├── Localization/ (ILocalizationService, Resource)
│   ├── Notifications/ (IEmailSender, EmailMessage)
│   ├── Jobs/ (IJobScheduler, IEmailJob, IMaintenanceJob)
│   └── Caching/ICacheableQuery.cs
├── Behaviours/                           # MediatR pipeline (see §5)
├── Common/
│   ├── Models/                           # ApiResponseDto, PagedApiResponseDto, MessageDto, Metadata, PagedRequest
│   ├── Facades/                          # Auth (Laravel-style static facade)
│   └── Extensions/                       # PaginationExtensions, ValidationRuleExtensions
├── DependencyInjection.cs                # AddApplication()
└── Auth/                                 # feature area
    ├── Authentication/                    # Login, RefreshToken, Logout, LogoutAll, GetMySessions
    ├── Users/{Commands,Queries}/...
    ├── Roles/{Commands,Queries}/...
    └── Permissions/Queries/...
```

### 3.3 Infrastructure (`src/Infrastructure`)

```
Infrastructure/
├── ApplicationDbContext.cs               # implements IApplicationDbContext
├── ApplicationDbInitializer.cs           # applies migrations on startup
├── Configurations/Auth/                  # IEntityTypeConfiguration<T> per entity
├── Data/
│   ├── ApplicationDbContextFactory.cs    # design-time factory for `dotnet ef`
│   └── Migrations/                        # CLI-generated migrations
├── Services/                             # Localization, PasswordHasher, CurrentUser, JwtToken, SmtpEmailSender, EmailJob, MaintenanceJob
├── Jobs/                                 # HangfireJobScheduler (IJobScheduler)
├── Authorization/                        # [HasPermission], policy provider, HangfireDashboardAuthorizationFilter
├── Middleware/                           # ExceptionHandlingMiddleware
├── Options/                              # AppOptions, DatabaseOptions, CacheOptions, MailOptions, JwtOptions
├── Seeders/                              # Laravel-style seeders (see §10)
└── DependencyInjection.cs                # AddInfrastructure() — uses Scrutor for service scanning
```

### 3.4 API (`src/API`)

```
API/
├── Controllers/
│   ├── ApiControllerBase.cs              # Sender (MediatR) + CurrentUserId (JWT claim)
│   └── Auth/                             # UsersController, RolesController, PermissionsController, AuthController
├── Extensions/                           # focused startup helpers (see §11)
├── config/                              # Laravel-style split configuration (see §7)
├── resources/lang/                      # YAML localization files: en.yml, ar.yml (see §7.1)
└── Program.cs                            # composition root
```

---

## 4. The standard response envelope

Every endpoint returns a consistent shape:

- `ApiResponseDto<T>` — `Success(value)` / `Failed(message)` for single results.
- `PagedApiResponseDto<T>` — paged lists; derives from `ApiResponseDto<List<T>>` and adds a `MetaData` block.
- `MessageDto` — message-only operations (assign/update/delete).

```jsonc
{
  "isSuccess": true,
  "data": [ { "id": 1, "nameEn": "...", "email": "..." } ],
  "metaData": { "resultSet": { "count": 137, "limit": 20, "offset": 0 } }
}
```

Failures are thrown as exceptions and mapped to the envelope + HTTP status by
[`ExceptionHandlingMiddleware`](../src/Infrastructure/Middleware/ExceptionHandlingMiddleware.cs):

| Exception | Status |
|-----------|--------|
| `FluentValidation.ValidationException` | 400 (with `errors[]`) |
| `BadRequestException` | 400 |
| `ItemNotFoundException` | 404 |
| `ForbiddenAccessException` | 403 |
| `TooManyRequestsException` | 429 |
| anything else | 500 (logged) |

---

## 5. CQRS request flow

```
Controller → MediatR Command → Handler → Domain entity + DbContext   (WRITES: SaveChangesAsync)
Controller → MediatR Query   → Handler → DbContext (AsNoTracking + .Select)  (READS)
```

Cross-cutting concerns live in the MediatR pipeline, registered in this order in
[`Application/DependencyInjection.cs`](../src/Application/DependencyInjection.cs):

1. `LoggingBehaviour` — logs request start/finish.
2. `ValidationBehaviour` — runs FluentValidation; throws `ValidationException` on failure.
3. `PerformanceBehaviour` — warns on slow requests.
4. `CachingBehaviour` — caches responses for queries implementing `ICacheableQuery`.
5. `AuditBehaviour` — audit trail for commands, attributed to the current user.

### Centralized pagination

Paged handlers filter → order → project → call `ToPagedResponseAsync(query)`. All counting and
page-bound clamping live in
[`PaginationExtensions`](../src/Application/Common/Extensions/PaginationExtensions.cs) and
[`PagedApiResponseFactory`](../src/Application/Common/Models/PagedApiResponseFactory.cs) — change
paging behaviour in one place. Defaults: page size 20, max 100.

---

## 6. Authentication & Authorization (RBAC)

**Model:** `User` ⇄ `Role` (many-to-many via `UserRole`) and `Role` ⇄ `Permission`
(many-to-many via `RolePermission`).

- **Login** ([`LoginCommand`](../src/Application/Auth/Authentication/Commands/Login/LoginCommand.cs))
  is by **email + password**, and delegates to `Auth.Attempt(...)` (see §6.1). It returns an
  `AuthTokenDto` with a short-lived **access token** (JWT carrying the user's `role` and `permission`
  claims) plus a long-lived **refresh token**.
- **Sessions & refresh tokens.** Each login creates a [`UserSession`](../src/Domain/Auth/Entities/UserSession.cs)
  row storing a **hashed** refresh token, expiry, and device/IP/user-agent. `POST /api/auth/refresh`
  rotates the refresh token and issues a new access token; `POST /api/auth/logout` revokes the current
  session; `POST /api/auth/logout-all` revokes every session of the user; `GET /api/auth/sessions`
  lists the caller's active sessions. Refresh-token lifetime is `Jwt:RefreshTokenExpiryDays`.
- **Authorize** endpoints with `[HasPermission(AuthPermissionCodes.UsersWrite)]`. The attribute
  encodes the permission into a policy name resolved on demand by
  [`PermissionPolicyProvider`](../src/Infrastructure/Authorization/PermissionPolicyProvider.cs), and
  [`PermissionAuthorizationHandler`](../src/Infrastructure/Authorization/PermissionAuthorizationHandler.cs)
  checks the caller's `permission` claims.

**Adding a permission:**

1. Add a constant to [`AuthPermissionCodes`](../src/Domain/Auth/Constants/AuthPermissionCodes.cs).
2. Add the catalog entry (code + EN/AR names) to
   [`PermissionRegistry`](../src/Domain/Auth/Constants/PermissionRegistry.cs).
3. Decorate the endpoint with `[HasPermission(AuthPermissionCodes.YourNew)]`.
4. Re-seed (the `PermissionSeeder` is additive) and assign it to a role.

JWT settings live under the `Jwt` config section → [`JwtOptions`](../src/Infrastructure/Options/JwtOptions.cs).
The `auth/login` endpoint is rate-limited (fixed window) via
[`AuthRateLimitingExtensions`](../src/API/Extensions/AuthRateLimitingExtensions.cs).

> **Security:** replace `Jwt:SecretKey` in `config/jwt.json` with a real 64+ char secret stored in
> user-secrets / a vault — never commit production secrets.

### 6.1 The Auth facade (Laravel-style)

A Laravel-like authentication facade exposes the current identity and credential operations. There are
two ways to use it:

1. **Inject** [`IAuth`](../src/Application/Abstractions/Security/IAuth.cs) into any handler/service (preferred — testable).
2. **Static** [`Auth`](../src/Application/Common/Facades/Auth.cs) facade for Laravel ergonomics
   (`Auth.Check`, `Auth.Id`, `Auth.User()`, …). It proxies to the request-scoped `IAuth`; the resolver
   is wired once at startup by `app.UseAuthFacade()`. Valid only inside an HTTP request.

| Member | Laravel equivalent | Description |
|--------|--------------------|-------------|
| `Auth.Id` | `Auth::id()` | Current user id, or `null`. |
| `Auth.Check` | `Auth::check()` | Is a user authenticated. |
| `Auth.Guest` | `Auth::guest()` | Is the caller anonymous. |
| `Auth.User(ct)` | `Auth::user()` | Loads the user (`CurrentUserDto`) from the DB. |
| `Auth.Roles` / `Auth.HasRole(r)` | `$user->hasRole()` | Roles from the token. |
| `Auth.Permissions` / `Auth.Can(p)` | `$user->can()` | Permissions from the token. |
| `Auth.Attempt(email, pw, ct)` | `Auth::attempt()` | Verify credentials → create a session + issue access/refresh tokens (or `null`). |
| `Auth.Validate(email, pw, ct)` | `Auth::validate()` | Verify credentials without issuing a token. |
| `Auth.Refresh(refreshToken, ct)` | — | Rotate the refresh token and issue a fresh access token (or `null`). |
| `Auth.Logout(refreshToken, ct)` | `Auth::logout()` | Revoke the session behind a refresh token. |
| `Auth.LogoutAll(ct)` | — | Revoke every active session of the current user. |

```csharp
// Inject IAuth (preferred):
public WhoAmIHandler(IAuth auth) { _auth = auth; }
var me = await _auth.User(ct);
if (_auth.Can(AuthPermissionCodes.UsersWrite)) { /* ... */ }

// Or the static facade:
if (Auth.Check) { var id = Auth.Id; }
var token = await Auth.Attempt(email, password, ct);
```

The backing [`AuthService`](../src/Infrastructure/Services/AuthService.cs) also implements
`ICurrentUserService`, so it is the single source of truth for "who is the caller" across the app.

> The **access token** is a stateless JWT (validate without a DB hit), while **refresh tokens** are
> server-side sessions (`UserSession`) — so `Auth.Logout()` / `Auth.LogoutAll()` truly revoke access by
> invalidating the session, even though the short-lived access token remains valid until it expires.

---

## 7. Configuration (Laravel-style `config/`)

Configuration is split into one file per concern under [`src/API/config/`](../src/API/config/) —
the .NET analog of Laravel's `config/*.php`. Each file is loaded in
[`Program.cs`](../src/API/Program.cs) with an optional per-environment override:

```
src/API/config/
├── app.json          # App: name, url, version, time zone, default + supported locales
├── database.json     # Database: connection string, timeouts, retry, EF diagnostics
├── cache.json        # Cache: driver (Memory|Redis), Redis connection, default expiration
├── mail.json         # Mail: driver (Log|Smtp), host/port/credentials, From identity
├── jwt.json          # Jwt: issuer, audience, secret, expiry
└── queue.json        # Queue: driver (Hangfire), dashboard, worker count, queues
```

Each file carries `//` comments documenting its options and the **available driver values** — the
.NET configuration JSON provider allows comments and trailing commas.

Loading (base file required; per-environment override in a `config/{Environment}/` subfolder, optional):

```csharp
foreach (var file in new[] { "app", "database", "cache", "mail", "jwt", "queue" })
{
    builder.Configuration
        .AddJsonFile($"config/{file}.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"config/{builder.Environment.EnvironmentName}/{file}.json", optional: true, reloadOnChange: true);
}
```

So `config/database.json` is the base and `config/Development/database.json`, `config/Production/mail.json`,
etc. override per environment.

Each section is bound to a strongly-typed **options** class in
[`src/Infrastructure/Options/`](../src/Infrastructure/Options/) and registered in
`AddInfrastructure`, so you inject `IOptions<MailOptions>` (etc.) rather than reading raw strings:

| Config file | Section | Options class |
|-------------|---------|---------------|
| `app.json` | `App` | `AppOptions` |
| `database.json` | `Database` | `DatabaseOptions` |
| `cache.json` | `Cache` | `CacheOptions` |
| `mail.json` | `Mail` | `MailOptions` |
| `jwt.json` | `Jwt` | `JwtOptions` |
| `queue.json` | `Queue` | `QueueOptions` |

- `app.json` holds general app settings (name, URL, version, time zone) and the localization locales.
  `App:SupportedLocales` + `App:DefaultLocale` drive **request localization** (`AddAppLocalization` →
  `UseRequestLocalization`), so responses are localized per request via `?culture=ar`, a culture cookie,
  or the `Accept-Language` header. `LocalizationService` loads messages from **YAML language files**
  under [`src/API/resources/lang/`](../src/API/resources/lang/) (`en.yml`, `ar.yml`) — see §7.1.
- `database.json` drives the EF Core `DbContext` (connection string, command timeout, retry-on-failure,
  detailed/sensitive diagnostics). It falls back to `ConnectionStrings:DefaultConnection` if blank.
- `cache.json` selects the cache provider: `Memory` (default) or `Redis` (set `Driver` + `RedisConnection`).

**Add a new config file:** create `config/<name>.json`, add `"<name>"` to the loop in `Program.cs`,
add a matching `<Name>Options` class, and `services.Configure<<Name>Options>(...)` in `AddInfrastructure`.
The `config/*.json` files are copied to the output directory via a `<Content Update=... CopyToOutputDirectory>`
item in the API csproj. The `resources/**/*.yml` language files are copied the same way.

> **Secrets:** `jwt.json` / `mail.json` ship with placeholder values. Keep real secrets out of source
> control — use `dotnet user-secrets`, environment variables, or a vault (all override the JSON files).

### 7.1 Localization (YAML language files)

User-facing messages live in per-language YAML files under
[`src/API/resources/lang/`](../src/API/resources/lang/), one file per locale:

```
src/API/resources/lang/
├── en.yml          # English messages
└── ar.yml          # Arabic messages
```

Each file maps a [`Resource`](../src/Application/Abstractions/Localization/Resource.cs) enum key to its
text:

```yaml
UserCreated: "User created successfully."
UserNotFound: "User not found."
```

[`LocalizationService`](../src/Infrastructure/Services/LocalizationService.cs) resolves
`GetMessage(Resource)` against the current request culture (`CultureInfo.CurrentUICulture`, set by
request localization), falling back to `en`, then to the enum name. Parsed files are cached after the
first read (`YamlDotNet`), so there is no per-request file I/O.

**Add a message:** add the key to the `Resource` enum, then add a matching line to **every** `*.yml`
file. **Add a language:** drop a `<code>.yml` file in `resources/lang/` and add the locale to
`App:SupportedLocales` in `config/app.json`.

---

## 8. Mail & notifications (SMTP)

Email is sent through the [`IEmailSender`](../src/Application/Abstractions/Notifications/IEmailSender.cs)
abstraction (Application) implemented by
[`SmtpEmailSender`](../src/Infrastructure/Services/SmtpEmailSender.cs) (Infrastructure, **MailKit**).

The `Mail:Driver` setting (config/mail.json) controls behaviour:

- `Log` *(default)* — renders the message to the logger instead of sending. Safe for local dev; no SMTP server needed.
- `Smtp` — connects to `Host:Port` (STARTTLS when `UseStartTls`), authenticates if a `Username` is set, and sends.

`IEmailSender` exposes both single and **bulk** sends (bulk reuses one SMTP connection):

```csharp
Task SendAsync(EmailMessage message, CancellationToken ct = default);
Task SendBulkAsync(IReadOnlyList<EmailMessage> messages, CancellationToken ct = default);
```

Inject and use it from any handler/service:

```csharp
public sealed class SendWelcomeEmailHandler(IEmailSender email) // ...
{
    await email.SendAsync(
        EmailMessage.To1(user.Email, "Welcome!", "<h1>Welcome aboard</h1>"), ct);

    // Bulk — one message per recipient so addresses aren't exposed to each other:
    var messages = recipients.Select(r => EmailMessage.To1(r, subject, html)).ToList();
    await email.SendBulkAsync(messages, ct);
}
```

> For high-volume or slow sends, push the work onto the background queue instead of sending inline —
> enqueue [`IEmailJob`](../src/Application/Abstractions/Jobs/IEmailJob.cs) (which wraps `IEmailSender`)
> via the job scheduler (see §9). This starter kit intentionally ships **no HTTP endpoint** for email;
> trigger sends from your own use cases/services.

---

## 9. Background jobs & queue (Hangfire)

Background processing uses **Hangfire** (SQL Server storage), configured from `config/queue.json`
([`QueueOptions`](../src/Infrastructure/Options/QueueOptions.cs)). Handlers depend on the
[`IJobScheduler`](../src/Application/Abstractions/Jobs/IJobScheduler.cs) abstraction — never on Hangfire
directly — so the queue engine can be swapped.

```csharp
public interface IJobScheduler
{
    string Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall);                 // fire-and-forget
    string Schedule<TJob>(Expression<Func<TJob, Task>> methodCall, TimeSpan delay);// delayed
    void AddOrUpdateRecurring<TJob>(string id, Expression<Func<TJob, Task>> call, string cron); // recurring
}
```

- **Jobs** are interfaces in [`Application/Abstractions/Jobs/`](../src/Application/Abstractions/Jobs/)
  (e.g. `IEmailJob`, `IMaintenanceJob`) implemented in `Infrastructure/Services/`. Hangfire resolves them
  from DI and serializes the method arguments into storage.
- **Enqueue** off the request thread:
  `_jobScheduler.Enqueue<IEmailJob>(j => j.SendBulkAsync(messages));`
- **Recurring** jobs are registered at startup in
  [`HangfireServiceExtensions`](../src/API/Extensions/HangfireServiceExtensions.cs) (a sample daily
  `maintenance-heartbeat` is included).
- **Dashboard** is mounted at `Queue:DashboardPath` (default `/hangfire`) when `Queue:DashboardEnabled`
  is true, guarded by
  [`HangfireDashboardAuthorizationFilter`](../src/Infrastructure/Authorization/HangfireDashboardAuthorizationFilter.cs)
  (open in Development; requires authentication otherwise).
- The Hangfire server (`AddHangfireServer`) runs in-process and processes the configured `Queue:Queues`.
  Hangfire creates its own SQL schema (`Queue:SchemaName`) on first run, so SQL Server must be reachable.

---

## 10. Seeding (Laravel-style)

Seeding mirrors Laravel's `DatabaseSeeder` + `$this->call(...)`:

```
Seeders/
├── Seeder.cs                  # base class; Context, Services, CancellationToken + Call<T>() / Call(params)
├── DatabaseSeeder.cs          # root entry — runs Permission → Role → User in order
├── Auth/PermissionSeeder.cs   # additive: inserts any missing permission from PermissionRegistry
├── Auth/RoleSeeder.cs         # Admin (all permissions) + User (read-only); idempotent
└── Users/UserSeeder.cs        # seeds the default admin and assigns the Admin role
```

Each seeder is **idempotent**. The runner
[`DatabaseSeederExtensions.SeedDatabaseAsync`](../src/Infrastructure/Seeders/DatabaseSeederExtensions.cs)
is invoked from startup (after migrations). Add a new seeder by subclassing `Seeder`, registering it
in `DatabaseSeeder.RunAsync()`, and resolving any dependencies through its constructor.

**Default admin (dev):** `admin@template-net10.local` / `ChangeMe!123` — change before any real use.

---

## 11. Startup composition

[`Program.cs`](../src/API/Program.cs) is a thin composition root; behaviour lives in focused
extensions under `src/API/Extensions/`:

```csharp
// Laravel-style split config: load config/{database,cache,mail,jwt}.json (+ env overrides)
builder.AddServiceDefaults();                                  // Aspire
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddClientCors(builder.Configuration, builder.Environment);
builder.Services.AddApiSwagger();
builder.Services.AddAuthRateLimiting();
builder.Services.AddApiAuthentication(builder.Configuration);  // JWT bearer
builder.Services.AddApplicationServices(builder.Configuration);// Application + Infrastructure DI

var app = builder.Build();
await app.Services.MigrateAndSeedAsync();                      // migrate + seed (logs & continues if DB down)
app.UseApiPipeline();                                          // exception mw → swagger → cors → ratelimit → auth → controllers
app.Run();
```

---

## 12. How to add a use case (cheat sheet)

**Write** — `Application/{Area}/{Feature}/Commands/{Action}/`:

| File | Implements |
|------|------------|
| `{Action}Command.cs` | `IRequest<ApiResponseDto<T>>` |
| `{Action}CommandHandler.cs` | `IRequestHandler<{Action}Command, ApiResponseDto<T>>` — EF Core + `SaveChangesAsync` |
| `{Action}CommandValidator.cs` | `AbstractValidator<{Action}Command>` |

**Read** — `Application/{Area}/{Feature}/Queries/{Action}/`:

| File | Implements |
|------|------------|
| `{Action}Query.cs` | `IRequest<ApiResponseDto<T>>` (or `PagedApiResponseDto<T>`) |
| `{Action}QueryHandler.cs` | reads via `AsNoTracking()` + `.Select(...)` projection |
| `{Action}QueryValidator.cs` | optional (only when input needs validation) |

Then add the controller action (inherits `ApiControllerBase`, declares `[HasPermission]` +
`[ProducesResponseType]`, calls only `Sender.Send(...)`). New entity? Add an
`IEntityTypeConfiguration<T>`, expose it on `IApplicationDbContext`/`ApplicationDbContext`, and
generate a migration (§13).

---

## 13. Common commands

```powershell
# Build / test
dotnet build Template-net10.slnx
dotnet test  Tests/Template-net10.UnitTests/Template-net10.UnitTests.csproj

# Run via Aspire (recommended)
dotnet run --project Template-net10.AppHost

# Run the API directly
dotnet run --project src/API

# EF migrations (startup project = API, migrations live in Infrastructure)
dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/API --output-dir Data/Migrations
dotnet ef database update           --project src/Infrastructure --startup-project src/API

# Project tooling — the `do` CLI (tools/Do)
dotnet run --project tools/Do -- key:generate --show   # generate a fresh Jwt:SecretKey into config/jwt.json
dotnet run --project tools/Do -- rename Acme.Shop       # rename project/folders/namespaces for a new project
```

---

## 14. Golden rules (do not break)

1. **No Repository pattern** — handlers use `IApplicationDbContext` directly.
2. **No business logic in handlers** — it lives in Domain entities.
3. **Query handlers are read-only** — `AsNoTracking()` + `.Select(...)`, never `SaveChangesAsync`.
4. **Command handlers own writes** — load/track entities and call `SaveChangesAsync`.
5. **No business services in controllers** — only `Sender.Send(...)`.
6. **Entities are externally immutable** — private ctor, private setters, `Create`/`Update`.
7. **No hardcoded user-facing text** — use `ILocalizationService` + `Resource`.
8. **Namespace == folder path**; strict folder layout.
9. **Every runtime DB object ships in an EF migration** (CLI-generated, never hand-authored).
10. **Domain depends on nothing**; Infrastructure implements Application/Domain abstractions.
11. **Build + tests must be green** before a task is considered complete.
