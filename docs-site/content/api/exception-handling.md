# Exception Handling

Domain and application exceptions are mapped to HTTP status codes and the standard response envelope by `ExceptionHandlingMiddleware`.

## Exception Mapping

| Exception | HTTP Status | Response |
|-----------|-------------|----------|
| `FluentValidation.ValidationException` | 400 | `errors[]` populated |
| `BadRequestException` | 400 | `message` set |
| `ItemNotFoundException` | 404 | `message` set |
| `ForbiddenAccessException` | 403 | `message` set |
| `TooManyRequestsException` | 429 | `message` set |
| Unhandled | 500 | Generic message (logged) |

## Domain Exceptions

Located in `Domain/Common/Exceptions/`:

- `BadRequestException` — business rule violation
- `ItemNotFoundException` — entity not found
- `ForbiddenAccessException` — authorization failure at domain level
- `TooManyRequestsException` — rate limit exceeded

Throw from domain entities or handlers; never return error envelopes manually for exceptional cases — let the middleware handle it.

## Example

```csharp
// In a handler
var user = await _context.Users.FindAsync(id, ct)
    ?? throw new ItemNotFoundException(_localization.GetMessage(Resource.UserNotFound));
```

Results in HTTP 404:

```json
{
  "isSuccess": false,
  "message": "User not found."
}
```

## Localization

User-facing exception messages use `ILocalizationService` with keys from the `Resource` enum — never hardcode English strings in handlers.

## Related

- [Validation](/docs/api/validation)
- [Response Envelope](/docs/api/response-envelope)
