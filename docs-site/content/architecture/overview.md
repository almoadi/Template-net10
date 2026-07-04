# Architecture Overview

Template-net10 is a **.NET 10** backend starter kit built on **Clean Architecture + CQRS + MediatR**, orchestrated with **.NET Aspire**. This page summarizes the technology stack and high-level design.

## Technology Stack

| Concern | Technology / Package |
|---------|----------------------|
| Runtime / SDK | .NET 10 |
| Web framework | ASP.NET Core |
| Orchestration | .NET Aspire (AppHost + ServiceDefaults) |
| CQRS / Mediator | MediatR |
| Validation | FluentValidation |
| ORM | Entity Framework Core + SQL Server |
| Auth | JWT Bearer + server-side refresh sessions |
| Password hashing | ASP.NET Identity PasswordHasher |
| Caching | Memory or Redis |
| Mail | MailKit (SMTP) or Log driver |
| Background jobs | Hangfire (SQL Server storage) |
| Localization | YAML files (YamlDotNet) |
| DI scanning | Scrutor |
| API docs | Swashbuckle / OpenAPI |
| Tests | NUnit, Moq, FluentAssertions |

**Central Package Management** is enabled: all package versions live in `Directory.Packages.props`.

## Dependency Rule

```
API  →  Application  →  Domain
 │                        ▲
 └────  Infrastructure  ──┘
```

- **Domain** depends on nothing (no EF, no MediatR, no ASP.NET).
- **Application** depends only on Domain.
- **Infrastructure** implements Application/Domain abstractions.
- **API** composes everything in `Program.cs`.

## CQRS Request Flow

```
Controller → MediatR Command → Handler → Domain entity + DbContext   (writes)
Controller → MediatR Query   → Handler → DbContext AsNoTracking    (reads)
```

Controllers never contain business logic — they only call `Sender.Send(...)`.

## Golden Rules

1. No Repository pattern — handlers use `IApplicationDbContext` directly.
2. No business logic in handlers — it lives in Domain entities.
3. Query handlers are read-only (`AsNoTracking()` + `.Select(...)`).
4. Entities are externally immutable (private ctor/setters, `Create`/`Update`).
5. No hardcoded user-facing text — use `ILocalizationService` + YAML files.
6. Every runtime DB object ships in a CLI-generated EF migration.

## When & How to Use It

This page is your map when you're deciding *where new code belongs*. Lean on it when:

- **You're adding a feature** — the request flow tells you to add a Command/Query in Application,
  put business rules on a Domain entity, and implement any external service in Infrastructure.
- **You're onboarding** — the dependency rule (`API → Application → Domain`, Infrastructure on the
  side) explains why the projects reference each other the way they do.
- **You're unsure which layer** — if it's a pure rule with no framework, it's Domain; if it
  coordinates a use case, it's Application; if it talks to a database/SMTP/queue, it's Infrastructure.
- **You're reviewing code** — the golden rules are the checklist: no repositories, no logic in
  handlers or controllers, read-only queries, immutable entities.

**Rule of thumb:** dependencies point inward, and the Domain never depends on a framework.

## Related

- [Clean Architecture & CQRS](/docs/architecture/clean-architecture)
- [MediatR Pipeline](/docs/architecture/mediatr-pipeline)
- [Extending the Template](/docs/architecture/extending-the-template)
