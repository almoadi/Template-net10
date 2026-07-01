# Extending the Template

This page collects the most common extension recipes when building on Template-net10.

## Add a Use Case (Endpoint Action)

1. Create folder `Application/{Area}/{Feature}/Commands|Queries/{Action}/`.
2. Add `{Action}Command|Query.cs`, `{Action}Handler.cs`, `{Action}Validator.cs`.
3. Add controller action on `ApiControllerBase` with `[HasPermission]` + `[ProducesResponseType]`.
4. Call `Sender.Send(...)` only — no business logic in the controller.

## Add an Entity

1. Create entity in `src/Domain/` (private ctor, static `Create`, instance methods).
2. Add `IEntityTypeConfiguration<T>` in `Infrastructure/Configurations/`.
3. Expose `DbSet<T>` on both `IApplicationDbContext` and `ApplicationDbContext`.
4. Generate migration:

   ```powershell
   dotnet ef migrations add AddMyEntity --project src/Infrastructure --startup-project src/API --output-dir Data/Migrations
   dotnet ef database update --project src/Infrastructure --startup-project src/API
   ```

## Add a Permission

1. Add constant to `AuthPermissionCodes`.
2. Add catalog entry (code + EN/AR names) to `PermissionRegistry`.
3. Decorate endpoint with `[HasPermission(AuthPermissionCodes.YourNew)]`.
4. Re-seed (PermissionSeeder is additive) and assign to a role.

## Add a User-Facing Message

1. Add key to the `Resource` enum in `Application/Abstractions/Localization/Resource.cs`.
2. Add matching line to **every** `resources/lang/*.yml` file (`en.yml`, `ar.yml`).

## Add a Config Section

1. Create `config/{name}.json` (+ optional per-env overrides).
2. Add `"{name}"` to the config loop in `Program.cs`.
3. Add `{Name}Options` class in `Infrastructure/Options/`.
4. Register with `services.Configure<{Name}Options>(...)` in `AddInfrastructure`.

## Add a Background Job

1. Define interface in `Application/Abstractions/Jobs/`.
2. Implement in `Infrastructure/Services/` (auto-registered by Scrutor).
3. Enqueue via `_jobScheduler.Enqueue<IMyJob>(j => j.RunAsync(args))`.

## Add an Email

Mail has no HTTP endpoint — call `IEmailSender` from your handler or service:

```csharp
await _emailSender.SendAsync(new EmailMessage
{
    To = user.Email,
    Subject = _localization.GetMessage(Resource.WelcomeEmailSubject),
    Body = "...",
}, ct);
```

## Related

- [Database Migrations](/docs/database/migrations)
- [Creating Jobs](/docs/queue/creating-jobs)
- [Adding Translations](/docs/localization/adding-translations)
