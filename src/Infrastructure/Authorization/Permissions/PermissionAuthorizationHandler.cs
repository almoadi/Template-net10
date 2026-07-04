using Microsoft.AspNetCore.Authorization;

namespace Template_net10.Infrastructure.Authorization.Permissions;

/// <summary>Succeeds when the caller carries a matching <c>permission</c> claim.</summary>
public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var hasPermission = context.User.Claims.Any(c =>
            c.Type == PermissionConstants.ClaimType &&
            string.Equals(c.Value, requirement.Permission, StringComparison.Ordinal));

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
