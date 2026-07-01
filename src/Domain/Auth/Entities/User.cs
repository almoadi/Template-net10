using Template_net10.Domain.Auth.Events;
using Template_net10.Domain.Common;

namespace Template_net10.Domain.Auth.Entities;

/// <summary>
/// Application user. Externally immutable: created through <see cref="Create"/> and mutated
/// only through behaviour methods so all state transitions stay inside the domain.
/// </summary>
public class User : BaseEntity, IEmitsCreatedEvent, IEmitsDeletedEvent
{
    private readonly List<UserRole> _userRoles = new();

    private User()
    {
    }

    public string NameEn { get; private set; } = string.Empty;

    public string NameAr { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

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

    public User Update(string? nameEn, string? nameAr, string? email, string? phone)
    {
        NameEn = nameEn ?? NameEn;
        NameAr = nameAr ?? NameAr;
        Email = email is null ? Email : NormalizeEmail(email);
        Phone = phone ?? Phone;
        UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Soft-deletes the user and deactivates the account.</summary>
    public override void SoftDelete()
    {
        base.SoftDelete();
        Deactivate();
    }

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

    public void ClearRoles()
    {
        _userRoles.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public void EmitCreatedEvent() => RaiseDomainEvent(new UserCreatedDomainEvent(Id, Email));

    public void EmitDeletedEvent() => RaiseDomainEvent(new UserDeletedDomainEvent(Id, Email));

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
