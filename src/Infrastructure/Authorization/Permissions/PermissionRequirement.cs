using Microsoft.AspNetCore.Authorization;

namespace Template_net10.Infrastructure.Authorization.Permissions;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permission) => Permission = permission;

    public string Permission { get; }
}
