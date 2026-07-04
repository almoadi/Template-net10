# CLAUDE.md

AI agent rules for **Template-net10**. This repository is a fully documented .NET 10 starter kit.
**The documentation is the source of truth — read it before writing code, and keep it up to date.**

## Read first (required, in order)

1. **[`AGENTS.md`](AGENTS.md)** — the primary operating guide: architecture, conventions, golden
   rules, and recipes. This is authoritative; if guidance conflicts, `AGENTS.md` wins.
2. **[`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md)** — long-form architecture reference.
3. **`docs-site/content/**`** — per-feature usage docs (authentication, authorization, caching,
   storage, encryption, hashing, excel, pdf, realtime/websockets, idempotency, helpers, docker, …).
   The rendered documentation site lives in `docs-site/`.

## Non-negotiable rules

- **Follow the docs.** Obey every convention and every **Golden rule** in `AGENTS.md`. When unsure,
  search `docs-site/content/**` and `docs/` before inventing a pattern.
- **Match existing code.** Clean Architecture + CQRS + MediatR; feature-organized folders; namespace
  equals folder path; framework code under `Services/`/`BuildingBlocks`, business features as modules.
- **Update the docs with every change.** When you add or change a feature, update
  `docs-site/content/**` (and `docs-site/src/lib/navigation.ts`); if conventions change, update
  `AGENTS.md` and `docs/ARCHITECTURE.md` too.
- **Green before done.** A task is complete only when `dotnet build Template-net10.slnx` has 0 errors
  and `dotnet test Tests/Template-net10.UnitTests/...` passes.
- **Do not introduce forbidden patterns** (see `AGENTS.md` → "What this is NOT" and "Golden rules"):
  no Repository pattern, no business logic in handlers/controllers, no hardcoded user-facing strings,
  no multi-tenancy, no hand-authored EF migrations.

Treat `AGENTS.md` as the canonical rule set; this file only points you to it.
