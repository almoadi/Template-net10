namespace Template_net10.Domain.Common;

/// <summary>Implemented by <see cref="BaseEntity"/>. EF hides rows where <see cref="DeletedAt"/> is set.</summary>
public interface ISoftDeletable
{
    DateTime? DeletedAt { get; }
}
