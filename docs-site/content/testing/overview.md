# Testing Overview

Unit tests live in `Tests/Template-net10.UnitTests/` using **NUnit**, **Moq**, and **FluentAssertions**.

## Run Tests

```powershell
dotnet test Tests/Template-net10.UnitTests/Template-net10.UnitTests.csproj
```

Tests build Domain, Application, Infrastructure, and the test project — not the API host (avoiding locked DLL issues when the API is running).

## Test Stack

| Package | Purpose |
|---------|---------|
| NUnit | Test framework |
| Moq | Mocking dependencies |
| FluentAssertions | Readable assertions |
| EFCore.InMemory | In-memory database for integration-style tests |
| coverlet | Code coverage |

## What to Test

| Layer | Focus |
|-------|-------|
| Domain | Entity behaviour, validation rules, state transitions |
| Application | Handler logic with mocked `IApplicationDbContext` |
| Infrastructure | Service implementations, middleware |

## Patterns

- Mock `IApplicationDbContext` for handler tests
- Use `FluentAssertions` for readable failure messages
- Test validators independently from handlers

## Coverage

Coverage output is configured via coverlet. Run with:

```powershell
dotnet test --collect:"XPlat Code Coverage"
```

## When & How to Use It

Tests are how you prove behavior and stop regressions. Write one when:

- **A Domain rule matters** — entity validation and state changes (can't create a user with a bad
  email, `SoftDelete()` sets `DeletedAt`). These are fast, pure unit tests with no database.
- **A handler has real logic** — mock `IApplicationDbContext` (or use the in-memory provider) to
  check a command saves the right thing or a query filters correctly.
- **You add a validator** — test it on its own, separately from the handler.
- **You fix a bug** — add a test that reproduces it first, so it can't come back silently.

What **not** to test: trivial getters or framework behavior. Keep tests focused on your logic.

**Typical loop:** make a change → run `dotnet test` → keep it green before moving on.

## Related

- [Running the App](/docs/getting-started/running-the-app)
- [Clean Architecture & CQRS](/docs/architecture/clean-architecture)
