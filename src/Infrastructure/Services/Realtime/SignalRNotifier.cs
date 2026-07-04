using Microsoft.AspNetCore.SignalR;
using Template_net10.Application.Abstractions.Realtime;

namespace Template_net10.Infrastructure.Services.Realtime;

/// <summary>SignalR-backed <see cref="IRealtimeNotifier"/> using the <see cref="NotificationsHub"/> context.</summary>
public sealed class SignalRNotifier : IRealtimeNotifier
{
    private readonly IHubContext<NotificationsHub> _hub;

    public SignalRNotifier(IHubContext<NotificationsHub> hub) => _hub = hub;

    public Task SendToAllAsync(string eventName, object payload, CancellationToken cancellationToken = default)
        => _hub.Clients.All.SendAsync(eventName, payload, cancellationToken);

    public Task SendToUserAsync(int userId, string eventName, object payload, CancellationToken cancellationToken = default)
        => _hub.Clients.User(userId.ToString()).SendAsync(eventName, payload, cancellationToken);

    public Task SendToUsersAsync(
        IReadOnlyList<int> userIds, string eventName, object payload, CancellationToken cancellationToken = default)
        => _hub.Clients.Users(userIds.Select(id => id.ToString()).ToList()).SendAsync(eventName, payload, cancellationToken);

    public Task SendToGroupAsync(string group, string eventName, object payload, CancellationToken cancellationToken = default)
        => _hub.Clients.Group(group).SendAsync(eventName, payload, cancellationToken);
}
