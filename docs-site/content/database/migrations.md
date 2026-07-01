# Migrations

Database schema changes must be generated via the EF Core CLI — never hand-author migration files.

## Add a Migration

```powershell
dotnet ef migrations add AddMyEntity `
  --project src/Infrastructure `
  --startup-project src/API `
  --output-dir Data/Migrations
```

## Apply Migrations

```powershell
dotnet ef database update `
  --project src/Infrastructure `
  --startup-project src/API
```

## Automatic Migration on Startup

`ApplicationDbInitializer` calls `Database.MigrateAsync()` when the API starts, so pending migrations are applied automatically in development.

## Design-Time Factory

`ApplicationDbContextFactory` in `Infrastructure/Data/` enables `dotnet ef` commands without running the full app.

## Workflow

1. Add or modify entity in Domain.
2. Add/update `IEntityTypeConfiguration<T>`.
3. Expose `DbSet<T>` on both contexts.
4. Run `dotnet ef migrations add ...`.
5. Review generated migration, then `dotnet ef database update`.

## Related

- [Database Overview](/docs/database/overview)
- [Extending the Template](/docs/architecture/extending-the-template)
