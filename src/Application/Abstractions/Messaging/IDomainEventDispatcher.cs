using Template_net10.Domain.Common;

namespace Template_net10.Application.Abstractions.Messaging;

/// <summary>Dispatches domain events to their registered handlers after SaveChanges.</summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
