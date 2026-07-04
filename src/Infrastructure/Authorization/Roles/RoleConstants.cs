namespace Template_net10.Infrastructure.Authorization.Roles;

internal static class RoleConstants
{
    /// <summary>Prefix used to encode a role requirement into an authorization policy name.</summary>
    public const string PolicyPrefix = "ROLE:";

    /// <summary>Separator between role names when several are encoded in one policy (any-of).</summary>
    public const char Separator = ',';
}
