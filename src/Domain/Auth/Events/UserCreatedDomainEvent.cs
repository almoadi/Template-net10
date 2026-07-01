using Template_net10.Domain.Common;

namespace Template_net10.Domain.Auth.Events;

/// <summary>Raised after a new <see cref="Entities.User"/> is persisted.</summary>
public sealed record UserCreatedDomainEvent(int UserId, string Email) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
