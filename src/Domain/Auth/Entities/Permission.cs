using Template_net10.Domain.Common;

namespace Template_net10.Domain.Auth.Entities;

/// <summary>
/// A fine-grained capability (e.g. <c>users.write</c>) that can be granted to a <see cref="Role"/>.
/// The <see cref="Code"/> is the stable identifier referenced by <c>[HasPermission(...)]</c>.
/// </summary>
public class Permission : BaseEntity
{
    private readonly List<RolePermission> _rolePermissions = new();

    private Permission()
    {
    }

    public string Code { get; private set; } = string.Empty;

    public string NameEn { get; private set; } = string.Empty;

    public string NameAr { get; private set; } = string.Empty;

    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    public static Permission Create(string code, string nameEn, string nameAr)
        => new()
        {
            Code = code,
            NameEn = nameEn,
            NameAr = nameAr,
        };

    public Permission Update(string? nameEn, string? nameAr)
    {
        NameEn = nameEn ?? NameEn;
        NameAr = nameAr ?? NameAr;
        UpdatedAt = DateTime.UtcNow;
        return this;
    }
}
