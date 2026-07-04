# API Architecture & Scaffolding Guide

This document describes the **architecture**, **folder structure**, and **technology stack** of the
Subscriptions backend API so a **new project can be scaffolded with the exact same architecture**.

It covers the backend (`.NET`) API only — not the Flutter app or the Next.js web client.

---

## 1. Technology Stack

> Always use the **latest stable** version of each package.

| Concern | Technology / Package |
|---------|----------------------|
| Runtime / SDK | .NET (latest) |
| Web framework | ASP.NET Core (`Microsoft.NET.Sdk.Web`) |
| CQRS / Mediator | `MediatR` (+ DI extensions) |
| Validation | `FluentValidation` (+ DI extensions) |
| ORM (reads + writes) | `Microsoft.EntityFrameworkCore` + `*.SqlServer` + `*.Design` |
| Database driver | `Microsoft.Data.SqlClient` |
| Auth | `Microsoft.AspNetCore.Authentication.JwtBearer`, `System.IdentityModel.Tokens.Jwt` |
| Identity (hashing) | `Microsoft.Extensions.Identity.Core` |
| Caching | `Microsoft.Extensions.Caching.Memory`, `*.StackExchangeRedis` |
| DI scanning | `Scrutor` |
| API docs | `Swashbuckle.AspNetCore`, `Microsoft.AspNetCore.OpenApi` |
| Health checks | `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore` |
| Tests | `NUnit` + `NUnit3TestAdapter`, `Moq`, `FluentAssertions`, `EFCore.InMemory`, `coverlet` |

> **Central Package Management** is enabled. All versions live in `Directory.Packages.props`
> (`<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`). Project files reference
> packages with `<PackageReference Include="..." />` and **no `Version` attribute**.

Common project properties:

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

---

## 2. Architecture Overview

**Monolithic Clean Architecture + CQRS + MediatR.**

Five projects total under one solution:

```
API  →  Application  →  Domain
         ↑
Infrastructure  →  (implements Application/Domain abstractions)
```

- **API** — host, controllers, startup wiring. Depends on Application + Infrastructure.
- **Application** — use-case orchestration: commands, queries, handlers, validators, DTOs, abstractions, pipeline behaviours. Depends on Domain.
- **Domain** — entities, enums, constants, domain exceptions, `IApplicationDbContext`. **No dependencies** on EF/MediatR/Infrastructure.
- **Infrastructure** — `ApplicationDbContext`, EF configurations, migrations, external service implementations. Depends on Application + Domain.
- **Tests** — `*.UnitTests` (NUnit).

### Dependency rule

Dependencies point **inward**. Domain is the center and depends on nothing. Infrastructure
implements interfaces declared in Application/Domain (dependency inversion).

### Request flow (strict)

```
Controller → MediatR Command → Handler → Domain entity + DbContext  (WRITES, EF Core)
Controller → MediatR Query   → Handler → DbContext (AsNoTracking)    (READS, EF Core)
```

---

## 3. Folder Structure

```
<Solution>/
├── <Solution>.slnx
├── Directory.Packages.props          # central package versions
├── src/
│   ├── API/                          # Host + controllers + startup extensions
│   │   ├── Controllers/
│   │   │   ├── ApiControllerBase.cs  # base controller (Sender, CurrentUserId)
│   │   │   └── {Area}/               # one controller folder per feature area
│   │   ├── Extensions/               # service registration + pipeline extensions
│   │   ├── Properties/
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── Program.cs
│   ├── Application/
│   │   ├── Abstractions/             # interfaces grouped by concern (Data, Security, Caching, ...)
│   │   ├── Behaviours/               # MediatR pipeline behaviours
│   │   ├── Common/                   # shared DTOs, models, extensions, resources
│   │   ├── Options/
│   │   ├── DependencyInjection.cs
│   │   └── {Area}/{Feature}/                            # e.g. Auth/Users
│   │       ├── Commands/
│   │       │   └── {Action}/                            # one folder per write use case, e.g. CreateUser
│   │       │       ├── {Action}Command.cs              # request: IRequest<ApiResponseDto<T>>
│   │       │       ├── {Action}CommandHandler.cs       # write logic (EF Core + SaveChangesAsync)
│   │       │       └── {Action}CommandValidator.cs     # FluentValidation rules
│   │       ├── Queries/
│   │       │   └── {Action}/                            # one folder per read use case, e.g. GetUserById
│   │       │       ├── {Action}Query.cs                # request: IRequest<ApiResponseDto<T>>
│   │       │       ├── {Action}QueryHandler.cs         # read logic (AsNoTracking + .Select)
│   │       │       └── {Action}QueryValidator.cs       # optional (only when input needs validation)
│   │       └── {Feature}Dto.cs                          # response DTO shared by the feature
│   ├── Domain/                       # entities, enums, constants, IApplicationDbContext
│   │   ├── Abstractions/             # IApplicationDbContext, domain interfaces
│   │   ├── Common/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── LengthConstants.cs
│   │   │   └── Exceptions/           # BadRequest, ItemNotFound, ForbiddenAccess, TooManyRequests
│   │   └── {Area}/
│   │       ├── Entities/
│   │       ├── Constants/
│   │       └── Enums/
│   └── Infrastructure/
│       ├── ApplicationDbContext.cs
│       ├── ApplicationDbInitializer.cs
│       ├── Configurations/{Area}/    # IEntityTypeConfiguration<T>
│       ├── Data/
│       │   └── Migrations/
│       ├── Services/                 # external service implementations
│       ├── Authorization/            # permission attributes + handlers
│       ├── Middleware/
│       ├── Options/
│       └── DependencyInjection.cs
├── Tests/<Solution>.UnitTests/
└── Database/{Area}/Tables/   # table definitions (mirrors migrations)
```

> **Namespace = folder path.** Example:
> `src/Application/Auth/Users/Commands/CreateUser/CreateUserCommand.cs`
> → `namespace <Solution>.Application.Auth.Users.Commands.CreateUser`.

### 3.1 How to create the files for one use case

Each use case (one endpoint action) is a **self-contained folder** with a fixed file set.
Never put two use cases in the same folder, and never mix command and query files.

**Write use case** — `Application/{Area}/{Feature}/Commands/{Action}/`:

| File | Purpose | Implements |
|------|---------|------------|
| `{Action}Command.cs` | Input model carried by MediatR | `IRequest<ApiResponseDto<T>>` |
| `{Action}CommandHandler.cs` | Orchestrates the write via EF Core | `IRequestHandler<{Action}Command, ApiResponseDto<T>>` |
| `{Action}CommandValidator.cs` | Validates the input (required) | `AbstractValidator<{Action}Command>` |

**Read use case** — `Application/{Area}/{Feature}/Queries/{Action}/`:

| File | Purpose | Implements |
|------|---------|------------|
| `{Action}Query.cs` | Input model carried by MediatR | `IRequest<ApiResponseDto<T>>` |
| `{Action}QueryHandler.cs` | Reads via `AsNoTracking()` + `.Select(...)` | `IRequestHandler<{Action}Query, ApiResponseDto<T>>` |
| `{Action}QueryValidator.cs` | Validates the input (only when needed) | `AbstractValidator<{Action}Query>` |

**Shared response model** — `Application/{Area}/{Feature}/{Feature}Dto.cs` — the DTO returned by the
feature's queries (and commands when they return data).

> Example for "create a user":
> `Application/Auth/Users/Commands/CreateUser/` → `CreateUserCommand.cs`,
> `CreateUserCommandHandler.cs`, `CreateUserCommandValidator.cs`.
> The matching controller action lives in `API/Controllers/Auth/UsersController.cs` and only calls
> `Sender.Send(command)`.

---

## 4. Core Patterns (copy these exactly)

### 4.1 Controller

Controllers inherit `ApiControllerBase`, declare permissions + response types, and only call
`Sender.Send(...)`. No business services, no DbContext, no SQL.

```csharp
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth/users")]
public sealed class UsersController : ApiControllerBase
{
    [HasPermission(AuthPermissionCodes.UsersWrite)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<int>))]
    public async Task<ActionResult<ApiResponseDto<int>>> Create([FromBody] CreateUserCommand command)
        => await Sender.Send(command);

    [HasPermission(AuthPermissionCodes.UsersWrite)]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<MessageDto>))]
    public async Task<ActionResult<ApiResponseDto<MessageDto>>> Update(
        [FromRoute] int id, [FromBody] UpdateUserCommand command)
    {
        command.Id = id;               // route value always wins
        return await Sender.Send(command);
    }
}
```

`ApiControllerBase` provides `Sender` (lazy MediatR `ISender`) and `CurrentUserId` (from JWT claim).

### 4.2 Command (write) — EF Core

```csharp
public sealed class CreateUserCommand : IRequest<ApiResponseDto<int>>
{
    public string NameEn { get; set; } = string.Empty;
    public string Phone  { get; set; } = string.Empty;
}

public sealed class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, ApiResponseDto<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILocalizationService _localizationService;

    public async Task<ApiResponseDto<int>> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var entity = User.Create(command.NameEn, command.NameAr, command.Phone); // business rule in entity
        _context.Users.Add(entity);
        await _context.SaveChangesAsync(ct);
        return ApiResponseDto<int>.Success(entity.Id);
    }
}
```

- Writes use `IApplicationDbContext` + EF Core only.
- **No business logic in handlers** — state changes live inside the entity.
- User-facing text comes from `ILocalizationService.GetMessage(Resource.*)`.

### 4.3 Query (read) — EF Core projection

```csharp
public sealed class GetUserByIdQueryHandler
    : IRequestHandler<GetUserByIdQuery, ApiResponseDto<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUserByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<ApiResponseDto<UserDto>> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var dto = await _context.Users
            .AsNoTracking()
            .Where(x => x.Id == query.Id)
            .Select(x => new UserDto { Id = x.Id, NameEn = x.NameEn, Phone = x.Phone })
            .FirstOrDefaultAsync(ct);

        return dto is null ? ApiResponseDto<UserDto>.Failed() : ApiResponseDto<UserDto>.Success(dto);
    }
}
```

- Reads use `IApplicationDbContext` with `AsNoTracking()` and `.Select(...)` projections to a DTO.
- Keep query handlers read-only — never call `SaveChangesAsync` from a query.

#### Paged read (centralized pagination)

Pagination is **centralized** in one reusable `IQueryable<T>` extension — like Laravel's
`->paginate()`. Handlers never write `Count` / `Skip` / `Take` / build metadata by hand; they
filter, order, project, then call `ToPagedResponseAsync(request)`.

**The helper (write once)** — `Application/Common/Extensions/PaginationExtensions.cs`:

```csharp
public static class PaginationExtensions
{
    /// <summary>Laravel-style paginate(): normalizes the page, counts the total,
    /// applies Skip/Take, and wraps the rows in a PagedApiResponseDto.</summary>
    public static async Task<PagedApiResponseDto<T>> ToPagedResponseAsync<T>(
        this IQueryable<T> source,
        IPagedRequest request,
        CancellationToken ct = default) where T : class
    {
        var (limit, offset) = PagedApiResponseFactory.Normalize(request); // clamp Limit/Offset

        var totalCount = await source.CountAsync(ct);

        var rows = await source
            .Skip(offset)
            .Take(limit)
            .ToListAsync(ct);

        return new PagedApiResponseDto<T>(Metadata.Create(totalCount, limit, offset), rows);
    }
}
```

**Every paged handler then becomes a one-liner pipeline:**

```csharp
public sealed class SearchUsersQuery : PagedRequest, IRequest<PagedApiResponseDto<UserDto>>
{
    public string? Search { get; set; }   // optional filter
}

public sealed class SearchUsersQueryHandler
    : IRequestHandler<SearchUsersQuery, PagedApiResponseDto<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchUsersQueryHandler(IApplicationDbContext context) => _context = context;

    public Task<PagedApiResponseDto<UserDto>> Handle(SearchUsersQuery query, CancellationToken ct)
        => _context.Users
            .AsNoTracking()
            .Where(x => string.IsNullOrWhiteSpace(query.Search) || x.NameEn.Contains(query.Search))
            .OrderBy(x => x.Id)
            .Select(x => new UserDto { Id = x.Id, NameEn = x.NameEn, Phone = x.Phone })
            .ToPagedResponseAsync(query, ct);   // <-- centralized pagination
}
```

- The query inherits `PagedRequest` (`Limit` + `Offset`) and returns `PagedApiResponseDto<T>`.
- All counting, page-bound clamping (`DefaultPageSize` / `MaxPageSize`), and metadata live **inside**
  `ToPagedResponseAsync` — change paging behaviour in one place.
- Always project with `.Select(...)` and add a deterministic `.OrderBy(...)` **before** calling the helper.

### 4.4 Validator — FluentValidation

```csharp
public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.NameEn).RequiredWithMaxLength(nameof(CreateUserCommand.NameEn), LengthConstants.L255);
        RuleFor(x => x.Phone).RequiredSaudiMobileWithCountryCode(nameof(CreateUserCommand.Phone));
    }
}
```

Validation runs automatically via a `ValidationBehaviour` in the MediatR pipeline.

### 4.5 Domain entity — externally immutable

```csharp
public class User : BaseEntity
{
    private User() { }                                 // private ctor

    public string NameEn { get; private set; } = "";   // private setters

    public static User Create(string nameEn, string nameAr, string phone) // factory
        => new() { NameEn = nameEn, NameAr = nameAr, Phone = phone, IsActive = true };

    public User Update(string? nameEn, ...)             // instance mutators
    {
        NameEn = nameEn ?? NameEn;
        UpdatedAt = DateTime.UtcNow;
        return this;
    }
}
```

Rules: `private` constructor, `private set`, static `Create(...)`, instance `Update(...)` / behaviour methods.

### 4.6 MediatR pipeline behaviours

Registered in order; cross-cutting concerns live here, not in handlers:

- `LoggingBehaviour`
- `ValidationBehaviour`
- `PerformanceBehaviour`
- `CachingBehaviour`
- `AuditBehaviour`

### 4.7 Program.cs (composition root)

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddClientCors(builder.Configuration, builder.Environment);
builder.Services.AddApiSwagger();
builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration); // Application + Infrastructure DI

var app = builder.Build();
await app.Services.MigrateAndSeedAsync();
app.UseApiPipeline();
app.Run();
```

Startup is composed from focused extension methods in `src/API/Extensions/`:
`ApplicationServiceExtensions`, `AuthenticationServiceExtensions`, `AuthRateLimitingExtensions`,
`CorsServiceExtensions`, `MiddlewarePipelineExtensions`, `SwaggerServiceExtensions`.

---

## 5. CQRS Read/Write Split

| Operation | Tool | Lives in | Notes |
|-----------|------|----------|-------|
| **Read (Query)** | `IApplicationDbContext` + EF Core (`AsNoTracking` + `.Select`) | Query handler | Read-only; never call `SaveChangesAsync` |
| **Write (Command)** | `IApplicationDbContext` + EF Core (`SaveChangesAsync`) | Command handler | State changes happen inside Domain entities |

Both reads and writes use EF Core through `IApplicationDbContext`. Queries project directly to a DTO
with `AsNoTracking()` and `.Select(...)`; commands load/track entities and persist with
`SaveChangesAsync`.

---

## 6. Standard Response Envelope

All endpoints return a consistent envelope:

- `ApiResponseDto<T>` — `Success(value)` / `Failed()` for single results.
- `PagedApiResponseDto<T>` — paged list results.
- `MessageDto` — message-only operations (update/delete).

`PagedApiResponseDto<T>` derives from `ApiResponseDto<List<T>>` and adds a `MetaData` block, so a
paged response carries the data list plus paging info:

```jsonc
{
  "isSuccess": true,
  "data": [ { "id": 1, "nameEn": "...", "phone": "..." } ],
  "metaData": {
    "resultSet": {
      "count": 137,   // total rows matching the filter (not just this page)
      "limit": 20,    // page size actually applied (after Normalize)
      "offset": 0     // rows skipped
    }
  }
}
```

Build it with the centralized `IQueryable<T>.ToPagedResponseAsync(request)` helper (§4.3) — never
assemble metadata by hand in a handler. The controller declares it via
`[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedApiResponseDto<UserDto>))]`.

---

## 7. Scaffolding a New Project (checklist)

1. **Create solution + 5 projects**

   ```powershell
   dotnet new sln -n <Solution>
   dotnet new webapi   -o src/API            -n <Solution>.API
   dotnet new classlib -o src/Application    -n <Solution>.Application
   dotnet new classlib -o src/Domain         -n <Solution>.Domain
   dotnet new classlib -o src/Infrastructure -n <Solution>.Infrastructure
   dotnet new nunit    -o Tests/<Solution>.UnitTests -n <Solution>.UnitTests
   dotnet sln add (Get-ChildItem -Recurse *.csproj)
   ```

2. **Wire project references**
   - `API` → `Application`, `Infrastructure`
   - `Application` → `Domain`
   - `Infrastructure` → `Application`, `Domain`
   - `Tests` → `Application`, `Domain`, `Infrastructure`

3. **Enable Central Package Management** — add `Directory.Packages.props` (copy the package list
   from §1), set all `<TargetFramework>net10.0</TargetFramework>`, `Nullable` + `ImplicitUsings` enabled.

4. **Domain** — add `BaseEntity`, `IApplicationDbContext`, area folders (`Entities/Constants/Enums`),
   domain exceptions.

5. **Application** — add `Abstractions/`, `Behaviours/`, `Common/` (DTOs, `ApiResponseDto`, resources),
   `Queries/`, and a `DependencyInjection.cs` registering MediatR + FluentValidation + behaviours.

6. **Infrastructure** — add `ApplicationDbContext`, `Configurations/`, `Services/`, `Authorization/`,
   `Middleware/`, migrations, and a `DependencyInjection.cs` (use `Scrutor` for assembly scanning).

7. **API** — add `Controllers/ApiControllerBase.cs`, `Extensions/*` startup helpers, JWT auth,
   Swagger, CORS, and the `Program.cs` composition root above.

8. **Database** — create `Database/<Area>/Tables/` and keep table definitions in sync with migrations.

---

## 8. Golden Rules (do not break)

1. No Repository pattern — handlers use `IApplicationDbContext` directly.
2. No business logic in handlers — it lives in Domain entities.
3. Query handlers are read-only — use `AsNoTracking()` + `.Select(...)`, never `SaveChangesAsync`.
4. Command handlers own writes — load/track entities and call `SaveChangesAsync`.
5. No business services in controllers — only `Sender.Send(...)`.
6. Entities are externally immutable — private ctor, private setters, `Create` / `Update`.
7. No hardcoded user-facing text — use `ILocalizationService`.
8. Namespace must equal folder path; strict folder layout (§3).
9. Every runtime DB object ships in an EF migration (CLI-generated, never hand-authored).
10. Build must pass with zero errors before a task is considered complete.
```