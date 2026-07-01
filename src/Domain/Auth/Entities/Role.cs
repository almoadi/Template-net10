using Template_net10.Domain.Auth.Events;
using Template_net10.Domain.Common;
using Template_net10.Domain.Common.Exceptions;

namespace Template_net10.Domain.Auth.Entities;

/// <summary>
/// A named collection of <see cref="Permission"/>s assignable to users. System roles
/// (<see cref="IsSystem"/>) are seeded and protected from deletion.
/// </summary>
public class Role : BaseEntity, IEmitsDeletedEvent
{
    private readonly List<RolePermission> _rolePermissions = new();
    private readonly List<UserRole> _userRoles = new();

    private Role()
    {
    }

    public string NameEn { get; private set; } = string.Empty;

    public string NameAr { get; private set; } = string.Empty;

    public bool IsSystem { get; private set; }

    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    public static Role Create(string nameEn, string nameAr, bool isSystem = false)
        => new()
        {
            NameEn = nameEn,
            NameAr = nameAr,
            IsSystem = isSystem,
        };

    public Role Update(string? nameEn, string? nameAr)
    {
        NameEn = nameEn ?? NameEn;
        NameAr = nameAr ?? NameAr;
        UpdatedAt = DateTime.UtcNow;
        return this;
    }

    /// <summary>Soft-deletes the role. System roles cannot be deleted.</summary>
    public override void SoftDelete()
    {
        if (IsSystem)
        {
            throw new BadRequestException("System roles cannot be deleted.");
        }

        base.SoftDelete();
    }

    public void SetPermissions(IEnumerable<Permission> permissions)
    {
        _rolePermissions.Clear();
        foreach (var permission in permissions)
        {
            _rolePermissions.Add(RolePermission.Create(this, permission));
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPermission(Permission permission)
    {
        var exists = _rolePermissions.Any(rp =>
            ReferenceEquals(rp.Permission, permission) ||
            (permission.Id != 0 && rp.PermissionId == permission.Id));

        if (!exists)
        {
            _rolePermissions.Add(RolePermission.Create(this, permission));
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void EmitDeletedEvent() => RaiseDomainEvent(new RoleDeletedDomainEvent(Id, NameEn));
}
