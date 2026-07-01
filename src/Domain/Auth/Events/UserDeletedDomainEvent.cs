using Template_net10.Domain.Common;

namespace Template_net10.Domain.Auth.Events;

/// <summary>Raised after a <see cref="Entities.User"/> is soft-deleted.</summary>
public sealed record UserDeletedDomainEvent(int UserId, string Email) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
