using Template_net10.Domain.Common;

namespace Template_net10.Domain.Auth.Entities;

/// <summary>
/// Application user. Externally immutable: created through <see cref="Create"/> and mutated
/// only through behaviour methods so all state transitions stay inside the domain.
/// </summary>
public class User : BaseEntity
{
    private readonly List<UserRole> _userRoles = new();

    // EF Core materialization ctor.
    private User()
    {
    }

    public string NameEn { get; private set; } = string.Empty;

    public string NameAr { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    /// <summary>Factory: the only way to bring a <see cref="User"/> into existence.</summary>
    public static User Create(string nameEn, string nameAr, string email, string phone, string passwordHash)
        => new()
        {
            NameEn = nameEn,
            NameAr = nameAr,
            Email = NormalizeEmail(email),
            Phone = phone,
            PasswordHash = passwordHash,
            IsActive = true,
        };

    /// <summary>Applies a partial update; null arguments leave the existing value untouched.</summary>
    public User Update(string? nameEn, string? nameAr, string? email, string? phone)
    {
        NameEn = nameEn ?? NameEn;
        NameAr = nameAr ?? NameAr;
        Email = email is null ? Email : NormalizeEmail(email);
        Phone = phone ?? Phone;
        UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Grants a role if the user does not already have it.</summary>
    public void AssignRole(Role role)
    {
        var exists = _userRoles.Any(ur =>
            ReferenceEquals(ur.Role, role) || (role.Id != 0 && ur.RoleId == role.Id));

        if (!exists)
        {
            _userRoles.Add(UserRole.Create(this, role));
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>Removes all role assignments (used before reassigning a fresh set).</summary>
    public void ClearRoles()
    {
        _userRoles.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
