using Microsoft.AspNetCore.Authorization;

namespace Template_net10.Infrastructure.Authorization.Permissions;

/// <summary>
/// Declares that the decorated action requires a specific permission code. Encodes the
/// permission into a policy name resolved on demand by <see cref="AuthorizationPolicyProvider"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        => Policy = $"{PermissionConstants.PolicyPrefix}{permission}";
}
