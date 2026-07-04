# HasRole Attribute

Coarse, role-level endpoint authorization via the `[HasRole]` attribute. It integrates with ASP.NET
Core policy-based authorization, just like [`[HasPermission]`](/docs/authorization/has-permission).

```csharp
[HasRole(AuthRoles.Admin)]
[HttpGet("admin/stats")]
public async Task<ApiResponseDto<StatsDto>> Stats(CancellationToken ct)
    => await Sender.Send(new GetStatsQuery(), ct);
```

## Any-of semantics

Pass multiple roles — the caller needs **at least one** of them:

```csharp
[HasRole(AuthRoles.Admin, AuthRoles.User)]
```

## How it works

1. `[HasRole("Admin")]` encodes the role(s) into a policy name (`ROLE:Admin`).
2. `AuthorizationPolicyProvider` creates the policy on demand.
3. `RoleAuthorizationHandler` checks the authenticated user's role claims in their JWT.

## HasRole vs. HasPermission

| Attribute | Granularity | Use when |
|-----------|-------------|----------|
| `[HasPermission]` | Fine-grained capability | Preferred for most endpoints |
| `[HasRole]` | Coarse role membership | Simple role gating (e.g. Admin-only areas) |

## Related

- [HasPermission](/docs/authorization/has-permission)
- [Roles](/docs/authorization/roles)
