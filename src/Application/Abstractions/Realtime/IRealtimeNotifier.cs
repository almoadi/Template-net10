namespace Template_net10.Application.Abstractions.Realtime;

/// <summary>
/// Pushes real-time messages to connected WebSocket clients (implemented with SignalR in
/// Infrastructure). Inject this in handlers/services to broadcast events — never depend on SignalR
/// directly. <paramref name="eventName"/> is the client-side handler name; <paramref name="payload"/>
/// is serialized to JSON.
/// </summary>
public interface IRealtimeNotifier
{
    /// <summary>Broadcasts to every connected client.</summary>
    Task SendToAllAsync(string eventName, object payload, CancellationToken cancellationToken = default);

    /// <summary>Sends to all active connections of a single user.</summary>
    Task SendToUserAsync(int userId, string eventName, object payload, CancellationToken cancellationToken = default);

    /// <summary>Sends to all active connections of the given users.</summary>
    Task SendToUsersAsync(IReadOnlyList<int> userIds, string eventName, object payload, CancellationToken cancellationToken = default);

    /// <summary>Sends to every connection that joined the named group.</summary>
    Task SendToGroupAsync(string group, string eventName, object payload, CancellationToken cancellationToken = default);
}
