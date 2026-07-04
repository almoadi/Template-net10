namespace Template_net10.Domain.Common;

/// <summary>
/// Base type for all persisted entities. Carries the surrogate key, audit timestamps,
/// soft-delete column (global EF scope), and optional domain events.
/// </summary>
public abstract class BaseEntity : ISoftDeletable, IActivatable
{
    private static readonly System.Globalization.UmAlQuraCalendar Hijri = new();

    private readonly List<IDomainEvent> _domainEvents = new();

    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime? _updatedAt;

    public int Id { get; protected set; }

    public DateTime CreatedAt
    {
        get => _createdAt;
        protected set
        {
            _createdAt = value;
            CreatedAtHijri = ToHijri(value);
        }
    }

    public DateTime? UpdatedAt
    {
        get => _updatedAt;
        protected set
        {
            _updatedAt = value;
            UpdatedAtHijri = value is { } updated ? ToHijri(updated) : null;
        }
    }

    public DateTime? DeletedAt { get; protected set; }

    public bool IsActive { get; protected set; } = true;

    public bool IsDeleted => DeletedAt is not null;

    /// <summary>Persisted <see cref="CreatedAt"/> in the Um Al-Qura (Hijri) calendar, formatted <c>yyyy/MM/dd</c>.</summary>
    public string CreatedAtHijri { get; protected set; } = ToHijri(DateTime.UtcNow);

    /// <summary>Persisted <see cref="UpdatedAt"/> in the Hijri calendar, or <c>null</c> when never updated.</summary>
    public string? UpdatedAtHijri { get; protected set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>Soft-deletes the row (Eloquent soft delete). Idempotent.</summary>
    public virtual void SoftDelete()
    {
        if (DeletedAt is not null)
        {
            return;
        }

        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Restores a soft-deleted row. Idempotent.</summary>
    public virtual void Restore()
    {
        if (DeletedAt is null)
        {
            return;
        }

        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
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

    private static string ToHijri(DateTime value)
        => $"{Hijri.GetYear(value):0000}/{Hijri.GetMonth(value):00}/{Hijri.GetDayOfMonth(value):00}";
}
