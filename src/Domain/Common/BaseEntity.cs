namespace Template_net10.Domain.Common;

/// <summary>
/// Base type for all persisted entities. Carries the surrogate key and audit timestamps.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; protected set; }

    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; protected set; }
}
