using Microsoft.Extensions.Logging;
using Template_net10.Application.Abstractions.Messaging;
using Template_net10.Domain.Auth.Events;

namespace Template_net10.Application.Auth.Users.Events;

/// <summary>Example side-effect handler for <see cref="UserCreatedDomainEvent"/> (structured log only).</summary>
public sealed class UserCreatedDomainEventHandler : IDomainEventHandler<UserCreatedDomainEvent>
{
    private readonly ILogger<UserCreatedDomainEventHandler> _logger;

    public UserCreatedDomainEventHandler(ILogger<UserCreatedDomainEventHandler> logger)
        => _logger = logger;

    public Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Domain event: UserCreated UserId={UserId} Email={Email}",
            domainEvent.UserId,
            domainEvent.Email);

        return Task.CompletedTask;
    }
}
