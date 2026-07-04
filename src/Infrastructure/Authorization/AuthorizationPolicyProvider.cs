using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Template_net10.Infrastructure.Authorization.Permissions;
using Template_net10.Infrastructure.Authorization.Roles;

namespace Template_net10.Infrastructure.Authorization;

/// <summary>
/// Materializes an authorization policy on demand for each <c>[HasPermission("...")]</c> and
/// <c>[HasRole("...")]</c> usage, so neither permissions nor roles need to be pre-registered as
/// named policies. Unknown policy names fall through to the default provider.
/// </summary>
public sealed class AuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallback;

    public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        => _fallback = new DefaultAuthorizationPolicyProvider(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(PermissionConstants.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName[PermissionConstants.PolicyPrefix.Length..];

            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(permission))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        if (policyName.StartsWith(RoleConstants.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var roles = policyName[RoleConstants.PolicyPrefix.Length..]
                .Split(RoleConstants.Separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new RoleRequirement(roles))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallback.GetPolicyAsync(policyName);
    }
}
