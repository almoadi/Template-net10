using Template_net10.Domain.Common;

namespace Template_net10.Domain.Auth.Events;

/// <summary>Raised after a <see cref="Entities.Role"/> is soft-deleted.</summary>
public sealed record RoleDeletedDomainEvent(int RoleId, string NameEn) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
