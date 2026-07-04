# HasPermission Attribute

Endpoint authorization uses the `[HasPermission]` attribute, which integrates with ASP.NET Core policy-based authorization.

## Usage

```csharp
[HasPermission(AuthPermissionCodes.UsersRead)]
[HttpGet("{id:int}")]
public async Task<ActionResult<ApiResponseDto<UserDto>>> GetById([FromRoute] int id)
    => await Sender.Send(new GetUserByIdQuery { Id = id });
```

## How It Works

1. `[HasPermission("users.read")]` encodes the permission into a policy name.
2. `PermissionPolicyProvider` creates policies on demand for each permission code.
3. `PermissionAuthorizationHandler` checks if the authenticated user has a matching `permission` claim in their JWT.
4. Missing or invalid token → 401; valid token but missing permission → 403.

## Components

| Component | Path |
|-----------|------|
| Attribute | `Infrastructure/Authorization/HasPermissionAttribute.cs` |
| Policy provider | `Infrastructure/Authorization/PermissionPolicyProvider.cs` |
| Handler | `Infrastructure/Authorization/PermissionAuthorizationHandler.cs` |
| Hangfire guard | `Infrastructure/Authorization/HangfireDashboardAuthorizationFilter.cs` |

## Hangfire Dashboard

The Hangfire dashboard at `/hangfire` uses a separate authorization filter requiring an authenticated admin user.

## AllowAnonymous

Auth login and refresh endpoints use `[AllowAnonymous]` instead — they are rate-limited separately.

## Related

- [RBAC Overview](/docs/authorization/overview)
- [Permissions](/docs/authorization/permissions)
