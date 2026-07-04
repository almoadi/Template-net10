# Rate Limiting

Authentication endpoints are protected by a fixed-window rate limiter to reduce brute-force attacks.

## Protected Endpoints

| Endpoint | Policy |
|----------|--------|
| `POST /api/v1/auth/login` | `Auth` rate limit |
| `POST /api/v1/auth/refresh` | `Auth` rate limit |

Both endpoints also have `[AllowAnonymous]` and are the only auth routes without a bearer token.

## Implementation

Rate limiting is configured in `AuthRateLimitingExtensions.cs` and applied via:

```csharp
[EnableRateLimiting(RateLimitingPolicies.Auth)]
[HttpPost("login")]
```

When exceeded, the middleware throws `TooManyRequestsException`, mapped to HTTP **429** by `ExceptionHandlingMiddleware`.

## Response on Rate Limit

```json
{
  "isSuccess": false,
  "message": "Too many requests. Please try again later."
}
```

## Related

- [Login](/docs/authentication/login)
- [Refresh Tokens](/docs/authentication/refresh-tokens)
- [Exception Handling](/docs/api/exception-handling)
