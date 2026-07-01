using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Template_net10.Application.Abstractions.Messaging;
using Template_net10.Domain.Common;

namespace Template_net10.Infrastructure.Data;

/// <summary>
/// After a successful SaveChanges, emits created/deleted events and dispatches domain events
/// (Eloquent Observer hooks).
/// </summary>
public sealed class DomainEventDispatchInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventDispatcher _dispatcher;
    private List<BaseEntity> _addedEntities = [];
    private List<BaseEntity> _softDeletedEntities = [];

    public DomainEventDispatchInterceptor(IDomainEventDispatcher dispatcher) => _dispatcher = dispatcher;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            _addedEntities = eventData.Context.ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity)
                .ToList();

            _softDeletedEntities = eventData.Context.ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Modified
                    && e.Property(nameof(BaseEntity.DeletedAt)).IsModified
                    && e.Entity.DeletedAt is not null)
                .Select(e => e.Entity)
                .ToList();
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var saved = await base.SavedChangesAsync(eventData, result, cancellationToken);

        if (eventData.Context is null || result == 0)
        {
            return saved;
        }

        foreach (var entity in _addedEntities)
        {
            if (entity is IEmitsCreatedEvent emitsCreated)
            {
                emitsCreated.EmitCreatedEvent();
            }
        }

        foreach (var entity in _softDeletedEntities)
        {
            if (entity is IEmitsDeletedEvent emitsDeleted)
            {
                emitsDeleted.EmitDeletedEvent();
            }
        }

        var entitiesWithEvents = eventData.Context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var events = entitiesWithEvents.SelectMany(e => e.DomainEvents).ToList();

        foreach (var entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }

        if (events.Count > 0)
        {
            await _dispatcher.DispatchAsync(events, cancellationToken);
        }

        _addedEntities = [];
        _softDeletedEntities = [];
        return saved;
    }
}
