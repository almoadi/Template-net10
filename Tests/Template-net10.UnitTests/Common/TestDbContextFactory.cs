using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Abstractions.Messaging;
using Template_net10.Domain.Common;
using Template_net10.Infrastructure;
using Template_net10.Infrastructure.Data;
using Template_net10.Infrastructure.Services;

namespace Template_net10.UnitTests.Common;

/// <summary>Builds an isolated in-memory <see cref="ApplicationDbContext"/> for handler tests.</summary>
internal static class TestDbContextFactory
{
    public static ApplicationDbContext Create(IDomainEventDispatcher? dispatcher = null)
    {
        dispatcher ??= new NoOpDomainEventDispatcher();

        var interceptor = new DomainEventDispatchInterceptor(dispatcher);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .AddInterceptors(interceptor)
            .Options;

        return new ApplicationDbContext(options);
    }
}

internal sealed class NoOpDomainEventDispatcher : IDomainEventDispatcher
{
    public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

internal sealed class CapturingDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly Action<IDomainEvent> _capture;

    public CapturingDomainEventDispatcher(Action<IDomainEvent> capture) => _capture = capture;

    public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            _capture(domainEvent);
        }

        return Task.CompletedTask;
    }
}
