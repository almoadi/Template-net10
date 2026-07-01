namespace Template_net10.Domain.Common;

/// <summary>
/// Base type for all persisted entities. Carries the surrogate key, audit timestamps,
/// soft-delete column (global EF scope), and optional domain events.
/// </summary>
public abstract class BaseEntity : ISoftDeletable, IActivatable
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public int Id { get; protected set; }

    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; protected set; }

    public DateTime? DeletedAt { get; protected set; }

    public bool IsActive { get; protected set; } = true;

    public bool IsDeleted => DeletedAt is not null;

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
}
