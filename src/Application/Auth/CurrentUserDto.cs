namespace Template_net10.Application.Auth;

/// <summary>Snapshot of the authenticated user returned by the Auth facade's <c>User()</c>.</summary>
public sealed class CurrentUserDto
{
    public int Id { get; init; }

    public string NameEn { get; init; } = string.Empty;

    public string NameAr { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public IReadOnlyList<string> Roles { get; init; } = [];

    public IReadOnlyList<string> Permissions { get; init; } = [];
}
