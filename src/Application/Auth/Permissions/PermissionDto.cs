namespace Template_net10.Application.Auth.Permissions;

public sealed class PermissionDto
{
    public int Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string NameEn { get; init; } = string.Empty;

    public string NameAr { get; init; } = string.Empty;
}
