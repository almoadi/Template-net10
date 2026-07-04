using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Template_net10.Infrastructure.Services.Realtime;

/// <summary>
/// Authenticated SignalR hub for server→client real-time messaging. Clients connect with a JWT
/// (SignalR passes it via the <c>access_token</c> query string on the WebSocket handshake). SignalR
/// maps connections to users by the <c>NameIdentifier</c> claim, so <c>Clients.User(userId)</c> works.
/// Clients can opt into named groups (e.g. a room or tenant channel).
/// </summary>
[Authorize]
public sealed class NotificationsHub : Hub
{
    /// <summary>Subscribes the caller's connection to a named group.</summary>
    public Task JoinGroup(string group) => Groups.AddToGroupAsync(Context.ConnectionId, group);

    /// <summary>Unsubscribes the caller's connection from a named group.</summary>
    public Task LeaveGroup(string group) => Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
}
