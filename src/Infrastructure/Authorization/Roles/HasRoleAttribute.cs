using Microsoft.AspNetCore.Authorization;

namespace Template_net10.Infrastructure.Authorization.Roles;

/// <summary>
/// Declares that the decorated action requires the caller to hold at least one of the given roles
/// (any-of). Encodes the roles into a policy name resolved on demand by
/// <see cref="AuthorizationPolicyProvider"/>. Prefer <c>[HasPermission]</c> for fine-grained checks;
/// use <c>[HasRole]</c> for coarse, role-level gating.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class HasRoleAttribute : AuthorizeAttribute
{
    public HasRoleAttribute(params string[] roles)
        => Policy = $"{RoleConstants.PolicyPrefix}{string.Join(RoleConstants.Separator, roles)}";
}
