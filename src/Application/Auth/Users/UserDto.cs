namespace Template_net10.Application.Auth.Users;

/// <summary>Response shape returned by the Users feature's queries (and data-returning commands).</summary>
public sealed class UserDto
{
    public int Id { get; init; }

    public string NameEn { get; init; } = string.Empty;

    public string NameAr { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Phone { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public IReadOnlyList<string> Roles { get; init; } = [];
}
