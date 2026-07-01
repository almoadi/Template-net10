# Introduction

**Template-net10** is a production-ready **.NET 10** backend starter kit — the foundation you copy into new projects. It is a monolith built on **Clean Architecture + CQRS + MediatR**, wired into **.NET Aspire**. Many conventions deliberately mirror **Laravel** (config folder, seeders, the `Auth` facade, YAML lang files).

Because it is a starter kit, the codebase favors **clarity, consistency, and completeness** over cleverness. New code should look like the existing code.

## Technology Stack

| Concern | Technology |
|---------|------------|
| Runtime | .NET 10 |
| Web framework | ASP.NET Core |
| Orchestration | .NET Aspire |
| CQRS | MediatR |
| Validation | FluentValidation |
| ORM | Entity Framework Core (SQL Server) |
| Auth | JWT + server-side refresh sessions |
| Caching | Memory or Redis |
| Mail | MailKit (SMTP) or Log driver |
| Jobs | Hangfire |
| Localization | YAML language files |
| Tests | NUnit |

## What You Get Out of the Box

- **RBAC** — users, roles, permissions with policy-based authorization
- **Authentication** — email + password login, JWT access tokens, refresh token rotation
- **Laravel-style config** — split JSON files with per-environment overrides
- **Response envelope** — consistent `ApiResponseDto<T>` for every endpoint
- **Seeders** — default admin user, roles, and permission catalog
- **Background jobs** — Hangfire with dashboard at `/hangfire`
- **Localization** — English and Arabic via YAML files

## Default Development Credentials

After seeding, you can log in with:

| Field | Value |
|-------|-------|
| Email | `admin@template-net10.local` |
| Password | `ChangeMe!123` |

> **Note:** Change these credentials before deploying to any shared environment.

## Solution Layout

```
Template-net10/
├── src/Domain/              # entities, enums, constants — no dependencies
├── src/Application/         # CQRS use cases, abstractions, behaviours
├── src/Infrastructure/    # EF Core, services, auth, seeders, jobs
├── src/API/                 # ASP.NET host, controllers, config/
├── Tests/                   # NUnit unit tests
├── tools/Do/                # `do` CLI (rename, key:generate)
├── Template-net10.AppHost/  # Aspire orchestrator
└── docs-site/               # this documentation site
```

## Next Steps

- [Installation](/docs/getting-started/installation) — create a new project from the template
- [Running the App](/docs/getting-started/running-the-app) — build, test, and run locally
- [Architecture Overview](/docs/architecture/overview) — understand the layers and conventions
