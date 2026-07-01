namespace Template_net10.Domain.Common;

/// <summary>
/// Entity emits a domain event after soft deletion (Eloquent <c>deleted</c> hook).
/// Invoked once <see cref="BaseEntity.DeletedAt"/> is set and the row is saved.
/// </summary>
public interface IEmitsDeletedEvent
{
    void EmitDeletedEvent();
}
