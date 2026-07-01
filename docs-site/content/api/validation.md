# Validation

Input validation uses **FluentValidation** integrated into the MediatR pipeline via `ValidationBehaviour`.

## How It Works

1. Create a validator class alongside the command or query:

```csharp
public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
```

2. Validators are auto-registered from the Application assembly in `AddApplication()`.
3. `ValidationBehaviour` runs all validators before the handler executes.
4. On failure, throws `FluentValidation.ValidationException`.

## HTTP Response on Validation Failure

`ExceptionHandlingMiddleware` maps validation failures to HTTP **400**:

```json
{
  "isSuccess": false,
  "message": "Validation failed.",
  "errors": [
    "Email is required.",
    "Password must not be empty."
  ]
}
```

## Shared Rules

Common validation rules live in `Application/Common/Extensions/ValidationRuleExtensions.cs`. String lengths should match `Domain/Common/LengthConstants.cs` so schema and validation stay in sync.

## When to Validate

| Request type | Validator |
|--------------|-----------|
| Commands with body input | Required |
| Queries with filter params | When inputs need constraints |
| Simple route-only queries | Optional |

## Related

- [MediatR Pipeline](/docs/architecture/mediatr-pipeline)
- [Exception Handling](/docs/api/exception-handling)
