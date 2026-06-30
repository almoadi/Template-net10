namespace Template_net10.Domain.Auth.Entities;

/// <summary>Join entity for the many-to-many between <see cref="Role"/> and <see cref="Permission"/>.</summary>
public class RolePermission
{
    private RolePermission()
    {
    }

    public int RoleId { get; private set; }

    public Role Role { get; private set; } = null!;

    public int PermissionId { get; private set; }

    public Permission Permission { get; private set; } = null!;

    public static RolePermission Create(Role role, Permission permission)
        => new() { Role = role, Permission = permission };
}
