# Clean Architecture & CQRS

Template-net10 separates concerns into four layers with a strict inward dependency rule. Each feature is implemented as a **use case** — a command (write) or query (read) handled by MediatR.

## Layer Responsibilities

### Domain (`src/Domain`)

Pure C#. Entities are **externally immutable**: private constructor, `private set` properties, a static `Create(...)` factory, and instance behaviour methods. All state transitions live here — never in handlers.

```
Domain/
├── Common/           # BaseEntity, LengthConstants, domain exceptions
└── Auth/
    ├── Entities/     # User, Role, Permission, UserRole, RolePermission, UserSession
    └── Constants/    # AuthPermissionCodes, AuthRoles, PermissionRegistry
```

### Application (`src/Application`)

Use-case orchestration. One self-contained folder per action:

```
Application/Auth/Users/Commands/CreateUser/
├── CreateUserCommand.cs
├── CreateUserHandler.cs
└── CreateUserValidator.cs
```

### Infrastructure (`src/Infrastructure`)

Implements abstractions: EF Core, JWT, mail, Hangfire, seeders, middleware, authorization.

### API (`src/API`)

Thin controllers that delegate to MediatR. Configuration in `config/`, localization in `resources/lang/`.

## Writes vs Reads

| Type | Handler pattern | DbContext |
|------|-----------------|-----------|
| **Command** | Load/track entity, call domain method, `SaveChangesAsync` | Tracked |
| **Query** | `AsNoTracking()` + `.Select(...)` projection | Read-only |

Paged queries end with `.ToPagedResponseAsync(query, ct)`.

## Adding a Use Case

1. Create folder `Application/{Area}/{Feature}/Commands|Queries/{Action}/`.
2. Add `{Action}Command|Query.cs`, `{Action}Handler.cs`, and `{Action}Validator.cs` (for input validation).
3. Add a controller action inheriting `ApiControllerBase` with `[HasPermission]` and `[ProducesResponseType]`.
4. Controller action calls only `Sender.Send(...)`.

## Business Logic in Domain

Never put business rules in handlers. Example pattern:

```csharp
// Domain entity
public static User Create(string nameEn, string email, ...) { ... }
public void AssignRole(Role role) { ... }

// Handler — orchestration only
var user = User.Create(command.NameEn, command.Email, ...);
_context.Users.Add(user);
await _context.SaveChangesAsync(ct);
```

## Related

- [Project Structure](/docs/getting-started/project-structure)
- [MediatR Pipeline](/docs/architecture/mediatr-pipeline)
