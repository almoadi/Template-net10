namespace Template_net10.Domain.Common;

/// <summary>
/// Entity emits a domain event after first persistence (Eloquent <c>created</c> hook).
/// Invoked by the EF save interceptor once <see cref="BaseEntity.Id"/> is assigned.
/// </summary>
public interface IEmitsCreatedEvent
{
    void EmitCreatedEvent();
}
