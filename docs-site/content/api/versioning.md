# API Versioning

The API is versioned with [Asp.Versioning](https://github.com/dotnet/aspnet-api-versioning) using a
**URL segment** — every endpoint lives under `/api/v{version}/...` (for example
`/api/v1/auth/login`). This keeps versions explicit and lets the API evolve without breaking existing
clients.

## How it works

Versioning is wired in `AddApiVersioningSupport()`
([`src/API/Extensions/ApiVersioningServiceExtensions.cs`](../../src/API/Extensions/ApiVersioningServiceExtensions.cs)):

- **Default version** `1.0` — requests that don't specify a version fall back to it
  (`AssumeDefaultVersionWhenUnspecified`).
- **Reported versions** — responses include an `api-supported-versions` header
  (`ReportApiVersions`).
- **Version readers** (in priority order):
  1. URL segment — `/api/v1/...` (primary, always visible)
  2. `X-Api-Version` header
  3. `api-version` query string

## Declaring a controller's version

Each controller declares its version and a route that carries the `{version:apiVersion}` token:

```csharp
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth/users")]
public sealed class UsersController : ApiControllerBase
{
    // /api/v1/auth/users
}
```

## Calling a versioned endpoint

```http
POST /api/v1/auth/login
Content-Type: application/json

{ "email": "admin@template-net10.local", "password": "ChangeMe!123" }
```

The header and query forms are also accepted (they select the version; the URL segment stays required
by the route):

```http
GET /api/v1/auth/users
X-Api-Version: 1.0
```

## Swagger

Swagger is version-aware: [`ConfigureSwaggerOptions`](../../src/API/Extensions/ConfigureSwaggerOptions.cs)
generates one OpenAPI document per discovered version, and the Swagger UI shows a dropdown (V1, V2, …).
New versions appear automatically — no Swagger changes needed.

## Adding a new version (e.g. v2)

1. Add the new version to the controller: `[ApiVersion("2.0")]` (a controller can serve several
   versions at once).
2. Map version-specific actions with `[MapToApiVersion("2.0")]`, or create a new controller whose
   route resolves to `/api/v2/...`.
3. Optionally mark the old version `[ApiVersion("1.0", Deprecated = true)]` — it still works and is
   flagged in the `api-supported-versions` header and in Swagger.

No Swagger or startup wiring changes are required — the version-aware API explorer discovers it.

## Related

- [Response Envelope](/docs/api/response-envelope)
- [Exception Handling](/docs/api/exception-handling)
- [Running the App](/docs/getting-started/running-the-app)
