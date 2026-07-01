using Microsoft.Extensions.Logging;
using Template_net10.Application.Abstractions.Messaging;
using Template_net10.Domain.Auth.Events;

namespace Template_net10.Application.Auth.Users.Events;

public sealed class UserDeletedDomainEventHandler : IDomainEventHandler<UserDeletedDomainEvent>
{
    private readonly ILogger<UserDeletedDomainEventHandler> _logger;

    public UserDeletedDomainEventHandler(ILogger<UserDeletedDomainEventHandler> logger) => _logger = logger;

    public Task HandleAsync(UserDeletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Domain event: UserDeleted UserId={UserId} Email={Email}",
            domainEvent.UserId,
            domainEvent.Email);

        return Task.CompletedTask;
    }
}
