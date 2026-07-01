# MediatR Pipeline

Every command and query passes through a MediatR pipeline of cross-cutting **behaviours** before reaching its handler. Behaviours are registered in `Application/DependencyInjection.cs` in this order:

## Pipeline Behaviours

| Order | Behaviour | Purpose |
|-------|-----------|---------|
| 1 | `LoggingBehaviour` | Logs request start and finish |
| 2 | `ValidationBehaviour` | Runs FluentValidation; throws `ValidationException` on failure |
| 3 | `PerformanceBehaviour` | Warns when requests exceed a time threshold |
| 4 | `CachingBehaviour` | Caches responses for queries implementing `ICacheableQuery` |
| 5 | `AuditBehaviour` | Records audit trail for commands, attributed to the current user |

## Validation

Validators live alongside handlers:

```
Application/Auth/Authentication/Commands/Login/
├── LoginCommand.cs
├── LoginHandler.cs
└── LoginValidator.cs
```

When validation fails, `ValidationBehaviour` throws `FluentValidation.ValidationException`, which `ExceptionHandlingMiddleware` maps to HTTP 400 with an `errors[]` array in the response envelope.

## Query Caching

Queries that implement `ICacheableQuery` are automatically cached by `CachingBehaviour`. Configure the cache driver in `config/cache.json` (`Memory` or `Redis`).

```csharp
public sealed class GetPermissionsQuery : IRequest<ApiResponseDto<...>>, ICacheableQuery
{
    public string CacheKey => "permissions:all";
    public TimeSpan? CacheExpiration => TimeSpan.FromMinutes(10);
}
```

## Controller Integration

Controllers inherit `ApiControllerBase`, which exposes:

- `Sender` — the MediatR `ISender`
- `CurrentUserId` — parsed from the JWT `sub` claim

```csharp
[HttpPost("login")]
public async Task<ActionResult<ApiResponseDto<AuthTokenDto>>> Login([FromBody] LoginCommand command)
    => await Sender.Send(command);
```

## Related

- [Validation](/docs/api/validation)
- [Caching Overview](/docs/caching/overview)
- [Exception Handling](/docs/api/exception-handling)
