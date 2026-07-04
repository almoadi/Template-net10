# AGENTS.md

Operating guide for AI coding agents working in this repository.

**Read this file first.** Use [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) for long-form reference (Auth facade table, mail/queue details, `do` CLI). **If guidance here conflicts with older code comments, this file and `docs/ARCHITECTURE.md` win.**

---

## Documentation is the source of truth

**Every AI agent must consult the documentation before writing code, and keep it up to date.** All
agent config files in this repo (`CLAUDE.md`, `GEMINI.md`, `.cursor/rules/`, `.cursorrules`,
`.windsurfrules`, `.github/copilot-instructions.md`) point here — `AGENTS.md` is the canonical rule set.

**Documentation index:**

| Source | What it covers |
|--------|----------------|
| [`AGENTS.md`](AGENTS.md) (this file) | Operating guide: architecture, conventions, golden rules, recipes |
| [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) | Long-form architecture reference |
| [`docs/api-architecture.md`](docs/api-architecture.md), [`docs/cli-do.md`](docs/cli-do.md), [`docs/template-install.md`](docs/template-install.md) | API layout, `do` CLI, template install |
| `docs-site/content/**` | Per-feature usage docs (auth, authorization, caching, storage, encryption, hashing, excel, pdf, realtime/websockets, idempotency, helpers, docker, …) rendered by the site in `docs-site/` |

**Rules for agents:**

1. **Read before you write** — search `docs-site/content/**` and `docs/` before inventing a pattern; follow the Golden rules below.
2. **Update docs with every change** — new/changed features must update `docs-site/content/**` (and `docs-site/src/lib/navigation.ts`); convention changes must update this file and `docs/ARCHITECTURE.md`.
3. **Green before done** — `dotnet build Template-net10.slnx` (0 errors) and `dotnet test` must pass.

---

## What this is

**Template-net10** is a production-ready **.NET 10** backend **starter kit** — the foundation copied into new projects. It is a monolith built on **Clean Architecture + CQRS + MediatR**, wired into **.NET Aspire**. Many conventions deliberately mirror **Laravel** (config folder, seeders, the `Auth` facade, YAML lang files, Eloquent-style scopes/events).

Because it is a starter kit, favor **clarity, consistency, and completeness** over cleverness. New code must look like the existing code.

### What this is NOT

Do **not** introduce patterns from other stacks unless they already exist here:

| Do not add | Use instead |
|------------|-------------|
| Repository pattern | Handlers inject `IApplicationDbContext` directly |
| Active Record (`entity.Save()`) | Command handler + `SaveChangesAsync` |
| Business logic in controllers/handlers | Domain entity methods (`Create`, `Update`, `SoftDelete`, …) |
| Business logic in domain-event handlers | Side effects only (email, cache, integration) |
| Hardcoded user-facing strings | `ILocalizationService` + `Resource` enum + YAML lang files |
| Multi-tenancy / `TenantId` | Not part of this template — do not add it |
| Livewire / Blade / Eloquent ORM | EF Core + MediatR (see § Eloquent-style infrastructure) |
| Hand-authored EF migrations | CLI-generated migrations only |

---

## Mental model — request flow

```
HTTP request
  → Controller (ApiControllerBase) — ONLY Sender.Send(...)
    → MediatR pipeline: Logging → Validation → Performance → Caching → Audit
      → Command handler (WRITE)  → load entity → domain method → SaveChangesAsync → domain events dispatch
      → Query handler  (READ)    → AsNoTracking() → scopes → .Select(...) → ToPagedResponseAsync (if paged)
  → ApiResponseDto<T> / PagedApiResponseDto<T>
  → ExceptionHandlingMiddleware maps domain/validation exceptions to HTTP status + envelope
```

**Writes** mutate tracked entities and call `SaveChangesAsync`. **Reads** never call `SaveChangesAsync` and always project with `.Select(...)`.

---

## Solution map

```
src/Domain              # entities, value objects, enums, constants, domain events, domain exceptions — depends on NOTHING
src/Application         # CQRS use cases, abstractions, behaviours, DTOs, query scopes — depends on Domain only
src/Infrastructure      # EF Core, services, auth, seeders, jobs, interceptors — implements Application/Domain abstractions
src/API                 # ASP.NET host, controllers, startup wiring, config/ + resources/
Tests/Template-net10.UnitTests   # NUnit + Moq + FluentAssertions + EF InMemory
tools/Do                         # `do` CLI — rename project, generate JWT key
Template-net10.AppHost           # Aspire orchestrator (preferred run target)
Template-net10.ServiceDefaults   # Aspire telemetry/health/resilience
```

Dependencies point **inward**: `API → Application → Domain`, and `Infrastructure → Application/Domain`. Domain references no framework packages.

`IApplicationDbContext` lives in `Application/Abstractions/Data` (not Domain) because it exposes EF's `DbSet<T>`.

### Naming

- **Namespaces / assembly names** use an underscore: `Template_net10.Application`
- **Project / folder names** use a hyphen: `Template-net10.Application.csproj`
- **Namespace must equal folder path**

---

## Technology stack (quick reference)

| Concern | Technology |
|---------|------------|
| Runtime | .NET 10 |
| Web | ASP.NET Core |
| Orchestration | .NET Aspire |
| CQRS | MediatR |
| Validation | FluentValidation |
| ORM | EF Core (SQL Server) |
| Auth | JWT bearer + refresh-token sessions |
| Cache | Memory or Redis (config driver) |
| Mail | MailKit (`Log` or `Smtp` driver) |
| Jobs | Hangfire (`IJobScheduler` abstraction) |
| Localization | YAML files (`YamlDotNet`) |
| DI scanning | Scrutor |
| Tests | NUnit, Moq, FluentAssertions, EF InMemory |
| Packages | Central Package Management — versions in `Directory.Packages.props` only |

---

## Layer responsibilities

### Domain (`src/Domain`)

Pure C#. No EF, MediatR, or ASP.NET references.

- **Entities** extend `BaseEntity` when persisted (see below).
- **Externally immutable**: private ctor, `private set`, static `Create(...)`, instance methods for all state changes.
- **Domain events** are records/classes implementing `IDomainEvent` in `{Area}/Events/`.
- **Exceptions** in `Domain/Common/Exceptions/`: `BadRequestException`, `ItemNotFoundException`, `ForbiddenAccessException`, `TooManyRequestsException`.
- **Constants** for permissions, roles, shared lengths (`LengthConstants`).

### Application (`src/Application`)

- **One folder per use case**: `Application/{Area}/{Feature}/Commands|Queries/{Action}/`
- **Abstractions** in `Application/Abstractions/` — interfaces only, grouped by concern.
- **DTOs** colocated with features or in `{Feature}Dto.cs`.
- **Query scopes** in `Application/Common/Extensions/QueryableScopeExtensions.cs` (generic) and `{Feature}ScopeExtensions.cs` (entity-specific).
- **Domain-event handlers** in `Application/{Area}/{Feature}/Events/` — side effects only.
- **MediatR behaviours** in `Application/Behaviours/` (do not reorder without reason).

### Infrastructure (`src/Infrastructure`)

- **EF**: `ApplicationDbContext` extends `GlobalFilteredDbContext`, configurations in `Configurations/`, migrations in `Data/Migrations/`.
- **Services** implement Application abstractions; registered via Scrutor scan in `DependencyInjection.cs`.
- **Interceptors**: `DomainEventDispatchInterceptor` runs after successful `SaveChangesAsync`.
- **Seeders**: Laravel-style under `Seeders/` — idempotent, additive.

### API (`src/API`)

- **Controllers** inherit `ApiControllerBase`, declare `[HasPermission]` + `[ProducesResponseType]`, call **only** `Sender.Send(...)`.
- **Config** in `config/*.json` (+ per-environment overrides).
- **Localization** in `resources/lang/{en,ar}.yml`.
- **Program.cs** is a thin composition root; behaviour lives in `Extensions/`.

---

## BaseEntity — every persisted entity

All main entities inherit [`BaseEntity`](src/Domain/Common/BaseEntity.cs):

| Column / member | Purpose |
|-----------------|---------|
| `Id` | Surrogate key |
| `CreatedAt`, `UpdatedAt` | Audit timestamps (UTC) |
| `DeletedAt` | Soft delete — set by `SoftDelete()`, cleared by `Restore()` |
| `IsActive` | Activation flag — `Activate()` / `Deactivate()` |
| `IsDeleted` | Computed: `DeletedAt is not null` |
| `DomainEvents` | Pending events cleared after dispatch |

**Global soft-delete filter** (automatic): `GlobalFilteredDbContext` applies `HasQueryFilter(e => e.DeletedAt == null)` to every `BaseEntity`. Soft-deleted rows are hidden unless you bypass the filter.

**EF mapping**: call `BaseEntityConfiguration.Configure(builder)` in each entity's `IEntityTypeConfiguration<T>`.

**Unique indexes** on business keys (email, phone, code, …) use a filtered index: `filter: "[DeletedAt] IS NULL"` so soft-deleted rows do not block re-use.

There is **no multi-tenancy** in this template. Do not add `TenantId`.

---

## Eloquent-style infrastructure

This kit mirrors selected [Laravel Eloquent](https://laravel.com/docs/eloquent) features using EF Core + Clean Architecture.

### Domain events (Eloquent Events / Observers)

| Eloquent | Template-net10 |
|----------|----------------|
| `created` | Entity implements `IEmitsCreatedEvent`; interceptor calls `EmitCreatedEvent()` after insert |
| `deleted` (soft) | Entity implements `IEmitsDeletedEvent`; interceptor calls `EmitDeletedEvent()` when `DeletedAt` is set |
| Custom events | Call `RaiseDomainEvent(new MyEvent(...))` on the entity before/around save |
| Observer class | `IDomainEventHandler<TEvent>` in Application — **side effects only** |

Flow: handler mutates entity → `SaveChangesAsync` → `DomainEventDispatchInterceptor` → `IDomainEventDispatcher` → handlers.

**Add a created/deleted event:**

1. Define event record in `Domain/{Area}/Events/`.
2. Implement `IEmitsCreatedEvent` and/or `IEmitsDeletedEvent` on the entity; emit from `EmitCreatedEvent()` / `EmitDeletedEvent()`.
3. Add `IDomainEventHandler<T>` in `Application/{Area}/{Feature}/Events/`.
4. Handlers are auto-registered by Scrutor — no manual DI wiring.

### Query scopes (Eloquent local + global scopes)

**Global scope** — soft delete (automatic on all `BaseEntity` queries):

```csharp
// Normal query — soft-deleted rows excluded automatically
_context.Users.AsNoTracking().Where(...);

// Include soft-deleted rows
_context.Users.WithTrashed();           // or .WithoutGlobalScopes()

// Only soft-deleted rows
_context.Users.OnlyTrashed();
```

**Generic local scopes** — [`QueryableScopeExtensions`](src/Application/Common/Extensions/QueryableScopeExtensions.cs):

| Method | Purpose |
|--------|---------|
| `Search(term, columns...)` | OR `Contains` across string columns |
| `OrderById` / `OrderByCreated` / … | Common ordering |
| `ActiveOnly()` / `InactiveOnly()` | Filter on `IsActive` (BaseEntity) |
| `WhereEquals` / `WhereIn` | Simple equality / IN filters |
| `WithTrashed` / `OnlyTrashed` | Soft-delete bypass helpers |

**Entity-specific scopes** — thin wrappers naming the searched columns:

- [`UserScopeExtensions`](src/Application/Auth/Users/UserScopeExtensions.cs): `SearchUsers`, `WithDeletedUsers`, `OnlyDeletedUsers`
- [`RoleScopeExtensions`](src/Application/Auth/Roles/RoleScopeExtensions.cs): `SearchRoles`, `ExcludeSystemRoles`, `WithDeletedRoles`, `OnlyDeletedRoles`

**Add scopes for a new entity:** create `{Entity}ScopeExtensions.cs` beside the feature; delegate to generic `Search(...)` with the right columns.

**Soft delete in domain:**

```csharp
user.SoftDelete();          // sets DeletedAt (+ User also deactivates)
await _context.SaveChangesAsync(ct);
// IEmitsDeletedEvent fires automatically if implemented
```

Override `SoftDelete()` on entities that need extra rules (e.g. `Role` blocks system roles).

---

## CQRS conventions

### Folder layout (one use case = one folder)

```
Application/Auth/Users/Commands/CreateUser/
├── CreateUserCommand.cs          # IRequest<ApiResponseDto<UserDto>>
├── CreateUserCommandHandler.cs   # IRequestHandler<...>
└── CreateUserCommandValidator.cs # AbstractValidator<CreateUserCommand>

Application/Auth/Users/Queries/SearchUsers/
├── SearchUsersQuery.cs           # IRequest<PagedApiResponseDto<UserDto>>
└── SearchUsersQueryHandler.cs    # AsNoTracking + scopes + Select + ToPagedResponseAsync
```

Validators are required for commands and for queries that accept input needing validation.

### MediatR pipeline order

Registered in [`Application/DependencyInjection.cs`](src/Application/DependencyInjection.cs):

1. `LoggingBehaviour`
2. `ValidationBehaviour` — throws `FluentValidation.ValidationException` → HTTP 400
3. `PerformanceBehaviour`
4. `CachingBehaviour` — queries implementing `ICacheableQuery`
5. `AuditBehaviour` — commands only, attributed to current user

### Response envelope

| Type | When |
|------|------|
| `ApiResponseDto<T>` | Single result or failure message |
| `PagedApiResponseDto<T>` | Paged lists — end with `.ToPagedResponseAsync(query, ct)` |
| `MessageDto` | Message-only operations (assign, delete, update status) |

Failures: throw domain exceptions or `ValidationException`; `ExceptionHandlingMiddleware` maps them to HTTP status + envelope.

| Exception | HTTP |
|-----------|------|
| `ValidationException` | 400 (with `errors[]`) |
| `BadRequestException` | 400 |
| `ItemNotFoundException` | 404 |
| `ForbiddenAccessException` | 403 |
| `TooManyRequestsException` | 429 |

Paged defaults: page size 20, max 100 — configured in `PaginationExtensions`.

---

## Authentication & authorization

- **Login**: email + password via `IAuth.Attempt(...)` or static `Auth.Attempt(...)`.
- **Tokens**: short-lived JWT access token + long-lived refresh token stored as hashed `UserSession`.
- **Authorization**: `[HasPermission(AuthPermissionCodes.X)]` on controller actions.
- **Current user in controllers**: `CurrentUserId` on `ApiControllerBase`.
- **Current user in handlers**: inject `IAuth` or `ICurrentUserService` (same backing service).

Default seeded admin (dev): `admin@template-net10.local` / `ChangeMe!123`.

See [`docs/ARCHITECTURE.md` §6](docs/ARCHITECTURE.md) for the full Auth facade member table.

---

## Configuration (Laravel-style `config/`)

Settings live in `src/API/config/`, one file per concern, bound to options classes in `src/Infrastructure/Options/`.

```csharp
// Program.cs composes configuration through one extension:
builder.AddSplitConfiguration();

// src/API/Extensions/ConfigurationExtensions.cs — add a new concern to this array:
private static readonly string[] ConfigFiles =
[
    "app", "database", "cache", "mail", "jwt", "queue", "logging",
    "cors", "storage", "features", "encryption", "idempotency", "auth", "socialite",
];
```

| File | Section | Options class | Drivers / notes |
|------|---------|---------------|-----------------|
| `app.json` | `App` | `AppOptions` | Locales, app name, timezone |
| `database.json` | `Database` | `DatabaseOptions` | Connection string, EF timeouts |
| `cache.json` | `Cache` | `CacheOptions` | `Memory` \| `Redis` |
| `mail.json` | `Mail` | `MailOptions` | `Log` \| `Smtp` |
| `jwt.json` | `Jwt` | `JwtOptions` | Issuer, secret, expiry |
| `queue.json` | `Queue` | `QueueOptions` | Hangfire dashboard, queues |
| `auth.json` | `Auth` | `AuthOptions` | Optional email verification, 2FA (email OTP), token lifetimes |

Secrets (`jwt.json` SecretKey, mail credentials, real DB strings) come from user-secrets / environment variables / a vault — **never committed**.

---

## Key features & entry points

| Feature | Where |
|---------|-------|
| **RBAC** | `Domain/Auth/Entities`; permission codes in `Domain/Auth/Constants` |
| **Auth facade** | `IAuth` + static `Auth` (Application); `AuthService` (Infrastructure) |
| **Social login** | `ISocialite` + static `Socialite` (Application); `SocialiteService` + `ISocialProviderDriver` (Google/Azure) in `Infrastructure/Services/Auth/Social`; token-based, `POST /api/auth/social/{provider}`; config `config/socialite.json` |
| **Authorization** | `[HasPermission]` / `[HasRole]` → `AuthorizationPolicyProvider` + `PermissionAuthorizationHandler` / `RoleAuthorizationHandler` |
| **Localization** | `resources/lang/{en,ar}.yml`; `LocalizationService`; `Resource` enum |
| **Mail** | `IEmailSender` → `SmtpEmailSender`; no HTTP endpoint — call from your use cases |
| **Queue / jobs** | `IJobScheduler` → Hangfire; job interfaces in `Application/Abstractions/Jobs`; dashboard `/hangfire` |
| **Seeders** | `Infrastructure/Seeders` — `DatabaseSeeder` → Permission → Role → User |
| **Domain events** | `DomainEventDispatchInterceptor` + `IDomainEventDispatcher` |
| **Global soft delete** | `GlobalFilteredDbContext` + `BaseEntity.DeletedAt` |
| **Query scopes** | `QueryableScopeExtensions` + entity-specific scope files |
| **Response envelope** | `ApiResponseDto<T>`, `PagedApiResponseDto<T>`, `MessageDto` |

---

## Recipes — how to extend

### Add a use case (endpoint action)

1. Create folder `Application/{Area}/{Feature}/Commands|Queries/{Action}/` with Command/Query, Handler, Validator (if needed).
2. **Write handler**: load/track via `_context`, call domain method, `SaveChangesAsync`.
3. **Read handler**: `AsNoTracking()` → scopes → `.Select(...)` → `ToPagedResponseAsync` if paged.
4. Add controller action: `[HasPermission]`, `[ProducesResponseType]`, `return await Sender.Send(new ...Command(...), ct);`

### Add an entity

1. Create entity in `Domain/{Area}/Entities/` extending `BaseEntity`.
2. Add `IEntityTypeConfiguration<T>` in `Infrastructure/Configurations/` — call `BaseEntityConfiguration.Configure(builder)`.
3. Add filtered unique indexes where needed (`HasFilter("[DeletedAt] IS NULL")`).
4. Expose `DbSet<T>` on **both** `IApplicationDbContext` and `ApplicationDbContext`.
5. Add `{Entity}ScopeExtensions.cs` if the entity is searched/filtered in queries.
6. Generate migration (CLI only):

```powershell
dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/API --output-dir Data/Migrations
dotnet ef database update        --project src/Infrastructure --startup-project src/API
```

### Add a permission

1. Constant in `AuthPermissionCodes`.
2. Catalog entry in `PermissionRegistry` (code + EN/AR names).
3. `[HasPermission(...)]` on the endpoint.
4. Re-seed (`PermissionSeeder` is additive) and assign to a role.

### Add a user-facing message

1. Key in `Resource` enum.
2. Matching line in **every** `resources/lang/*.yml` (`en.yml`, `ar.yml`).

### Add a config section

1. `config/{name}.json` (+ optional `config/{Environment}/{name}.json`).
2. Add `"{name}"` to the `ConfigFiles` array in `Extensions/ConfigurationExtensions.cs`.
3. `{Name}Options` in `Infrastructure/Options/`.
4. `services.Configure<{Name}Options>(...)` in `AddInfrastructure`.

### Add a background job

1. Interface in `Application/Abstractions/Jobs/`.
2. Implementation in `Infrastructure/Services/` (Scrutor auto-registers).
3. Enqueue: `_jobScheduler.Enqueue<IMyJob>(j => j.RunAsync(args))`.

### Add a domain event + handler

1. Event record in `Domain/{Area}/Events/`.
2. `RaiseDomainEvent(...)` on entity, or implement `IEmitsCreatedEvent` / `IEmitsDeletedEvent`.
3. `IDomainEventHandler<TEvent>` in `Application/` — email, cache, integration only; **no business rules**.

---

## Testing

Tests live in `Tests/Template-net10.UnitTests/` (NUnit).

| What to test | Where / how |
|--------------|-------------|
| Domain entity rules | `Tests/.../Domain/` — pure unit tests, no EF |
| Query scopes / global filters | In-memory EF via test `DbContext` extending `GlobalFilteredDbContext` (see `ScopeTestDbContextFactory`) |
| Handlers | Mock `IApplicationDbContext` or use InMemory DB |
| Domain event dispatch | `DomainEventDispatcherTests`, `DomainEventDispatchInterceptorTests` |

```powershell
dotnet test Tests/Template-net10.UnitTests/Template-net10.UnitTests.csproj
```

Add tests when behaviour is non-trivial. Do not add tests that only assert the obvious.

---

## Build, test, run (Windows / PowerShell)

```powershell
dotnet build Template-net10.slnx
dotnet test  Tests/Template-net10.UnitTests/Template-net10.UnitTests.csproj
dotnet run   --project Template-net10.AppHost     # preferred — via Aspire
dotnet run   --project src/API                    # API only
```

**A task is not complete until `dotnet build` is green (0 errors) and tests pass.**

### Gotcha: locked DLLs while the API is running

If the API is running, a full-solution build may fail with `MSB3027`/`MSB3021` on `src/API/bin/...`. Stop the app, or build to a temp output dir, or rely on `dotnet test` (which does not lock the API bin).

### EF migrations (CLI only — never hand-author)

Current schema ships as a single init migration: `20260701135453_InitialCreate`. After model changes:

```powershell
dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/API --output-dir Data/Migrations
dotnet ef database update        --project src/Infrastructure --startup-project src/API
```

If you squash migrations during early development: delete all files in `Data/Migrations/`, then `dotnet ef migrations add InitialCreate ...`.

---

## Project tooling — `do` CLI

```powershell
dotnet run --project tools/Do -- rename Acme.Shop       # rebrand namespaces, folders, JWT issuer
dotnet run --project tools/Do -- key:generate --show    # rotate Jwt:SecretKey in config/jwt.json
```

Full reference: [`docs/cli-do.md`](docs/cli-do.md).

---

## Golden rules (do not break)

1. **No Repository pattern** — handlers use `IApplicationDbContext` directly.
2. **No business logic in handlers** — it lives in Domain entities.
3. **Query handlers are read-only** — `AsNoTracking()` + `.Select(...)`, never `SaveChangesAsync`.
4. **Command handlers own writes** — load/track entities and call `SaveChangesAsync`.
5. **No business services in controllers** — only `Sender.Send(...)`.
6. **Entities are externally immutable** — private ctor, private setters, `Create`/`Update`/behaviour methods.
7. **No hardcoded user-facing text** — use `ILocalizationService` + `Resource` + YAML lang files.
8. **Namespace == folder path**; one use case per folder; never mix command and query files in one folder.
9. **Domain depends on nothing**; Infrastructure implements Application/Domain abstractions.
10. **Every runtime DB change ships in a CLI-generated EF migration** — never hand-author.
11. **Domain-event handlers are for side effects only** — business rules stay in entities.
12. **Soft delete via `SoftDelete()`** — do not hard-delete `BaseEntity` rows unless there is a strong reason.
13. **Secrets never committed** — config is per-concern JSON with per-environment overrides.
14. **No multi-tenancy** — do not add `TenantId` or tenant global filters.
15. **Build + tests must be green** before a task is done.

---

## Task completion checklist

Before marking work done, verify:

- [ ] Code follows layer rules and existing naming/folder layout
- [ ] Business logic is in Domain, not handlers/controllers/event handlers
- [ ] Reads use `AsNoTracking()` + projection; writes call `SaveChangesAsync`
- [ ] New user-facing text added to `Resource` + all YAML lang files
- [ ] New permissions added to constants, registry, and seeded
- [ ] EF migration generated if schema changed
- [ ] `dotnet build Template-net10.slnx` — 0 errors
- [ ] `dotnet test Tests/Template-net10.UnitTests/...` — all pass
