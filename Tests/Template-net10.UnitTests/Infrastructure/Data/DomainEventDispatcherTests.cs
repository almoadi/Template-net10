using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Template_net10.Application.Abstractions.Messaging;
using Template_net10.Domain.Auth.Events;
using Template_net10.Infrastructure.Services;

namespace Template_net10.UnitTests.Infrastructure.Data;

[TestFixture]
public sealed class DomainEventDispatcherTests
{
    [Test]
    public async Task Invokes_registered_handler_for_event_type()
    {
        var handler = new SpyUserCreatedHandler();
        var services = new ServiceCollection();
        services.AddSingleton<IDomainEventHandler<UserCreatedDomainEvent>>(handler);
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        await using var provider = services.BuildServiceProvider();

        var dispatcher = provider.GetRequiredService<IDomainEventDispatcher>();
        var domainEvent = new UserCreatedDomainEvent(42, "user@example.com");

        await dispatcher.DispatchAsync([domainEvent]);

        handler.Invoked.Should().BeTrue();
        handler.LastEvent.Should().Be(domainEvent);
    }

    private sealed class SpyUserCreatedHandler : IDomainEventHandler<UserCreatedDomainEvent>
    {
        public bool Invoked { get; private set; }

        public UserCreatedDomainEvent? LastEvent { get; private set; }

        public Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            Invoked = true;
            LastEvent = domainEvent;
            return Task.CompletedTask;
        }
    }
}
