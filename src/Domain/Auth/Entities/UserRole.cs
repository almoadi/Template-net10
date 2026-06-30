namespace Template_net10.Domain.Auth.Entities;

/// <summary>Join entity for the many-to-many between <see cref="User"/> and <see cref="Role"/>.</summary>
public class UserRole
{
    private UserRole()
    {
    }

    public int UserId { get; private set; }

    public User User { get; private set; } = null!;

    public int RoleId { get; private set; }

    public Role Role { get; private set; } = null!;

    public static UserRole Create(User user, Role role)
        => new() { User = user, Role = role };
}
