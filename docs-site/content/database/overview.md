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

Implemented by `ApplicationDbContext` in Infrastructure.

## Handler Patterns

**Write (command):**

```csharp
var user = User.Create(...);
_context.Users.Add(user);
await _context.SaveChangesAsync(ct);
```

**Read (query):**

```csharp
return await _context.Users
    .AsNoTracking()
    .Where(x => x.Id == id)
    .Select(x => new UserDto { ... })
    .FirstOrDefaultAsync(ct);
```

## Configurations

EF entity configurations live in `Infrastructure/Configurations/Auth/` as `IEntityTypeConfiguration<T>` classes.

## Startup

`ApplicationDbInitializer` applies pending migrations and runs seeders on startup.

## Related

- [Migrations](/docs/database/migrations)
- [Seeders](/docs/database/seeders)
- [Database Configuration](/docs/configuration/database)
