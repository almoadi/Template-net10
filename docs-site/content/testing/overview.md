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

## Related

- [Running the App](/docs/getting-started/running-the-app)
- [Clean Architecture & CQRS](/docs/architecture/clean-architecture)
