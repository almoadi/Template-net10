namespace Template_net10.Domain.Common;

/// <summary>
/// Marker for domain lifecycle events raised by entities (Eloquent Observer equivalent).
/// Handlers live in Application and perform side effects only — no business rules.
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
