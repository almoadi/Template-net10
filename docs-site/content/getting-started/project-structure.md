# Project Structure

Template-net10 follows **Clean Architecture** with strict dependency rules. Understanding the folder layout helps you know where to add new code.

## Solution Map

```
Template-net10/
├── Template-net10.slnx
├── Directory.Packages.props       # central NuGet versions
├── docs/                          # legacy markdown (see docs-site/ for the docs app)
├── docs-site/                     # React documentation site
├── src/
│   ├── Domain/                    # entities, value objects, enums, constants
│   ├── Application/               # CQRS use cases, abstractions, behaviours
│   ├── Infrastructure/            # EF Core, services, auth, seeders, jobs
│   └── API/                       # ASP.NET host, controllers, config/, resources/
├── Tests/Template-net10.UnitTests/
├── tools/Do/                      # `do` CLI
├── Template-net10.AppHost/        # Aspire orchestrator
└── Template-net10.ServiceDefaults/
```

## Dependency Rule

Dependencies point **inward**:

```
API  →  Application  →  Domain
 │                        ▲
 └────  Infrastructure  ──┘
```

| Layer | Depends on | Contains |
|-------|------------|----------|
| **Domain** | Nothing | Entities, enums, constants, domain exceptions |
| **Application** | Domain | Commands, queries, handlers, validators, abstractions |
| **Infrastructure** | Application, Domain | EF Core, services, auth, seeders, middleware |
| **API** | Application, Infrastructure | Controllers, Program.cs, config/, resources/ |

## Application Layer Layout

Each use case lives in a self-contained folder:

```
Application/
├── Auth/
│   ├── Authentication/
│   │   ├── Commands/Login/
│   │   │   ├── LoginCommand.cs
│   │   │   ├── LoginHandler.cs
│   │   │   └── LoginValidator.cs
│   │   └── Queries/GetMySessions/
│   ├── Users/
│   └── Roles/
├── Behaviours/          # MediatR pipeline (logging, validation, caching, audit)
├── Abstractions/        # interfaces (IAuth, IEmailSender, IJobScheduler, …)
└── Common/              # DTOs, extensions, facades
```

## Naming Conventions

| Context | Format | Example |
|---------|--------|---------|
| Project / folder names | Hyphen | `Template-net10.Application` |
| Namespaces / assemblies | Underscore | `Template_net10.Application` |
| Namespace must equal folder path | — | `Application/Auth/Users/...` → `Template_net10.Application.Auth.Users` |

## Key Files

| File | Purpose |
|------|---------|
| `src/API/Program.cs` | Composition root — config loading, middleware pipeline |
| `src/Infrastructure/DependencyInjection.cs` | Infrastructure service registration |
| `src/Application/DependencyInjection.cs` | MediatR + FluentValidation registration |
| `src/Infrastructure/ApplicationDbContext.cs` | EF Core DbContext |
| `src/Infrastructure/Seeders/DatabaseSeeder.cs` | Root seeder orchestrator |

## Related

- [Clean Architecture & CQRS](/docs/architecture/clean-architecture)
- [Extending the Template](/docs/architecture/extending-the-template)
