namespace Template_net10.Domain.Auth.Constants;

/// <summary>
/// Permission codes used by <c>[HasPermission(...)]</c> on controller actions. Keep these
/// in sync with the permissions seeded into the database (see <see cref="PermissionRegistry"/>).
/// </summary>
public static class AuthPermissionCodes
{
    public const string UsersRead = "users.read";
    public const string UsersWrite = "users.write";

    public const string RolesRead = "roles.read";
    public const string RolesWrite = "roles.write";

    public const string PermissionsRead = "permissions.read";
}
