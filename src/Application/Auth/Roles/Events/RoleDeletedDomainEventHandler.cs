using Microsoft.Extensions.Logging;
using Template_net10.Application.Abstractions.Messaging;
using Template_net10.Domain.Auth.Events;

namespace Template_net10.Application.Auth.Roles.Events;

public sealed class RoleDeletedDomainEventHandler : IDomainEventHandler<RoleDeletedDomainEvent>
{
    private readonly ILogger<RoleDeletedDomainEventHandler> _logger;

    public RoleDeletedDomainEventHandler(ILogger<RoleDeletedDomainEventHandler> logger) => _logger = logger;

    public Task HandleAsync(RoleDeletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Domain event: RoleDeleted RoleId={RoleId} NameEn={NameEn}",
            domainEvent.RoleId,
            domainEvent.NameEn);

        return Task.CompletedTask;
    }
}
