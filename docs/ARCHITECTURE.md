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
    ├── Entities/                         # User, Role, Permission, UserRole, RolePermission
    └── Constants/                        # AuthPermissionCodes, AuthRoles, PermissionRegistry
```

### 3.2 Application (`src/Application`)

Use-case orchestration. One **self-contained folder per use case**.

```
Application/
├── Abstractions/                         # interfaces grouped by concern
│   ├── Data/IApplicationDbContext.cs
│   ├── Security/ (ICurrentUserService, IPasswordHasher, IJwtTokenService)
│   ├── Localization/ (ILocalizationService, Resource)
│   └── Caching/ICacheableQuery.cs
├── Behaviours/                           # MediatR pipeline (see §5)
├── Common/
│   ├── Models/                           # ApiResponseDto, PagedApiResponseDto, MessageDto, Metadata, PagedRequest
│   └── Extensions/                       # PaginationExtensions, ValidationRuleExtensions
├── DependencyInjection.cs                # AddApplication()
└── Auth/                                 # feature area
    ├── Authentication/Commands/Login/
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
├── Services/                             # LocalizationService, PasswordHasherService, CurrentUserService, JwtTokenService
├── Authorization/                        # [HasPermission], requirement, handler, policy provider
├── Middleware/                           # ExceptionHandlingMiddleware
├── Options/                              # JwtOptions
├── Seeders/                              # Laravel-style seeders (see §7)
└── DependencyInjection.cs                # AddInfrastructure() — uses Scrutor for service scanning
```

### 3.4 API (`src/API`)

```
API/
├── Controllers/
│   ├── ApiControllerBase.cs              # Sender (MediatR) + CurrentUserId (JWT claim)
│   └── Auth/                             # UsersController, RolesController, PermissionsController, AuthController
├── Extensions/                           # focused startup helpers (see §8)
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
  is by **email + password**. The handler flattens the user's roles → permissions and embeds each
  permission code as a `permission` claim in the signed JWT.
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

> **Security:** replace `Jwt:SecretKey` in `appsettings.json` with a real 64+ char secret stored in
> user-secrets / a vault — never commit production secrets.

---

## 7. Seeding (Laravel-style)

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

## 8. Startup composition

[`Program.cs`](../src/API/Program.cs) is a thin composition root; behaviour lives in focused
extensions under `src/API/Extensions/`:

```csharp
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

## 9. How to add a use case (cheat sheet)

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
generate a migration (§10).

---

## 10. Common commands

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
```

---

## 11. Golden rules (do not break)

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
