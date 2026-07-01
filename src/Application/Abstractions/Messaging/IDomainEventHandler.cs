using Template_net10.Domain.Common;

namespace Template_net10.Application.Abstractions.Messaging;

/// <summary>
/// Handles a single domain event type (Eloquent Observer equivalent).
/// Side effects only — business rules stay in Domain entities.
/// </summary>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
