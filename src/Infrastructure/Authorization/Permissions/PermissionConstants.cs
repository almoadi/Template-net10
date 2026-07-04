namespace Template_net10.Infrastructure.Authorization.Permissions;

internal static class PermissionConstants
{
    /// <summary>Claim type used to carry permission codes in the JWT.</summary>
    public const string ClaimType = "permission";

    /// <summary>Prefix used to encode a permission requirement into an authorization policy name.</summary>
    public const string PolicyPrefix = "PERMISSION:";
}
