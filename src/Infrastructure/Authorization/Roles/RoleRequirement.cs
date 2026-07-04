using Microsoft.AspNetCore.Authorization;

namespace Template_net10.Infrastructure.Authorization.Roles;

public sealed class RoleRequirement : IAuthorizationRequirement
{
    public RoleRequirement(IReadOnlyList<string> roles) => Roles = roles;

    /// <summary>Any one of these roles satisfies the requirement.</summary>
    public IReadOnlyList<string> Roles { get; }
}
