# Database Overview

Template-net10 uses **Entity Framework Core** directly from handlers — there is no Repository pattern.

## IApplicationDbContext

The DbContext abstraction lives in Application (because it exposes `DbSet<T>`):

```csharp
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    // ...
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

Implemented by `ApplicationDbContext` in Infrastructure (extends `GlobalFilteredDbContext`).

## BaseEntity & global scopes

Every persisted entity extends [`BaseEntity`](../../src/Domain/Common/BaseEntity.cs), which carries:

| Column | Purpose |
|--------|---------|
| `DeletedAt` | Soft-delete global filter (hidden when set) |
| `IsActive` | Local `ActiveOnly()` scope via `IActivatable` |

The soft-delete global filter applies automatically to all `BaseEntity` types.

**Soft delete:** `entity.SoftDelete()` then `SaveChangesAsync`. Bypass with `.WithTrashed()` / `.OnlyTrashed()`.

## Handler Patterns

**Write (command):**

```csharp
var user = User.Create(...);
_context.Users.Add(user);
await _context.SaveChangesAsync(ct);
// Created/deleted domain events dispatch via DomainEventDispatchInterceptor
```

**Read (query):**

```csharp
return await _context.Users
    .AsNoTracking()
    .SearchUsers(term)
    .ActiveOnly()
    .OrderById()
    .Select(x => new UserDto { ... })
    .FirstOrDefaultAsync(ct);
```

**Roles:**

```csharp
_context.Roles
    .SearchRoles(term)
    .ExcludeSystemRoles()
    .OrderById()
```

## Generic local scopes

[`QueryableScopeExtensions`](../../src/Application/Common/Extensions/QueryableScopeExtensions.cs) work on any entity:

`Search`, `OrderById`, `ActiveOnly`, `WhereEquals`, `WhereIn`, `WhereId`, `CreatedAfter`, `WithoutGlobalScopes`, `WithTrashed`, `OnlyTrashed`, …

## Eloquent equivalents

| Laravel Eloquent | EF Core in this project |
|------------------|-------------------------|
| Observer `created` / `deleted` | `IEmitsCreatedEvent` / `IEmitsDeletedEvent` + handlers |
| Global soft delete | `BaseEntity.DeletedAt` + `GlobalFilteredDbContext` |
| Local scope `scopeX` | `QueryableScopeExtensions` |
| User / Role scopes | `UserScopeExtensions` / `RoleScopeExtensions` |
| `withTrashed()` / `onlyTrashed()` | `WithTrashed()` / `OnlyTrashed()` |

See [Architecture §10](/docs/architecture#10-eloquent-style-ef-infrastructure) for full details.

## Configurations

EF entity configurations live in `Infrastructure/Configurations/Auth/` as `IEntityTypeConfiguration<T>` classes.
Shared columns use `BaseEntityConfiguration.Configure(builder)`.

## Startup

`ApplicationDbInitializer` applies pending migrations and runs seeders on startup.

## When & How to Use It

The database layer is where your data lives, and you talk to it straight from handlers. In
everyday work you'll:

- **Write data** — in a command handler, build a domain entity (`User.Create(...)`), add it to the
  `DbContext`, and call `SaveChangesAsync`. Created/deleted events fire automatically.
- **Read data** — in a query handler, start from a `DbSet`, add `.AsNoTracking()`, apply scopes,
  and `.Select(...)` into a DTO. Query handlers never save changes.
- **Search and filter cleanly** — reuse scopes like `SearchUsers(term)`, `ActiveOnly()`, and
  `OrderById()` instead of repeating LINQ everywhere.
- **Delete without losing history** — call `entity.SoftDelete()`; the row is hidden by the global
  filter but stays in the table. Bring it back with `Restore()`.
- **See deleted rows on purpose** — add `.WithTrashed()` (include them) or `.OnlyTrashed()` (just
  the deleted ones), for example on an admin "recycle bin" screen.

**Rule of thumb:** commands change data and save; queries only read and project. Business rules
live on the entity, not in the handler.

## Related

- [Migrations](/docs/database/migrations)
- [Seeders](/docs/database/seeders)
- [Database Configuration](/docs/configuration/database)
