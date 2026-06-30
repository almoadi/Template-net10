namespace Template_net10.Domain.Auth.Constants;

/// <summary>
/// Canonical catalog of every permission the application knows about. The seeder reads this
/// list to create/refresh the <c>Permissions</c> table, so adding a new capability is a
/// one-line change here plus a re-seed.
/// </summary>
public static class PermissionRegistry
{
    public sealed record Definition(string Code, string NameEn, string NameAr);

    public static IReadOnlyList<Definition> All { get; } =
    [
        new(AuthPermissionCodes.UsersRead, "View users", "عرض المستخدمين"),
        new(AuthPermissionCodes.UsersWrite, "Manage users", "إدارة المستخدمين"),
        new(AuthPermissionCodes.RolesRead, "View roles", "عرض الأدوار"),
        new(AuthPermissionCodes.RolesWrite, "Manage roles", "إدارة الأدوار"),
        new(AuthPermissionCodes.PermissionsRead, "View permissions", "عرض الصلاحيات"),
    ];
}
