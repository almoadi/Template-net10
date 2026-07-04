using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Template_net10.Infrastructure.Authorization.Roles;

/// <summary>Succeeds when the caller carries at least one matching <see cref="ClaimTypes.Role"/> claim.</summary>
public sealed class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        var hasRole = context.User.Claims.Any(c =>
            c.Type == ClaimTypes.Role &&
            requirement.Roles.Any(role => string.Equals(c.Value, role, StringComparison.Ordinal)));

        if (hasRole)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
