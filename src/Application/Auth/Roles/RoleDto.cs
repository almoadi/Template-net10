namespace Template_net10.Application.Auth.Roles;

public sealed class RoleDto
{
    public int Id { get; init; }

    public string NameEn { get; init; } = string.Empty;

    public string NameAr { get; init; } = string.Empty;

    public bool IsSystem { get; init; }

    public IReadOnlyList<string> Permissions { get; init; } = [];
}
