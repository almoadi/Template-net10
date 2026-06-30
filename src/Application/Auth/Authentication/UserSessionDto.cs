namespace Template_net10.Application.Auth.Authentication;

/// <summary>A server-side authentication session belonging to the current user.</summary>
public sealed class UserSessionDto
{
    public int Id { get; init; }

    public string? Device { get; init; }

    public string? IpAddress { get; init; }

    public string? UserAgent { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime LastActivityAtUtc { get; init; }

    public DateTime ExpiresAtUtc { get; init; }
}
