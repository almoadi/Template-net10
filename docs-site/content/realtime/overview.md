# Real-time (WebSockets)

Push live updates to connected clients over WebSockets, backed by
[SignalR](https://learn.microsoft.com/aspnet/core/signalr/introduction). Inject `IRealtimeNotifier`
in handlers/services to broadcast — you never touch SignalR directly.

## IRealtimeNotifier

```csharp
public interface IRealtimeNotifier
{
    Task SendToAllAsync(string eventName, object payload, CancellationToken ct = default);
    Task SendToUserAsync(int userId, string eventName, object payload, CancellationToken ct = default);
    Task SendToUsersAsync(IReadOnlyList<int> userIds, string eventName, object payload, CancellationToken ct = default);
    Task SendToGroupAsync(string group, string eventName, object payload, CancellationToken ct = default);
}
```

## Broadcasting from a handler

```csharp
public sealed class CreateOrderHandler(IApplicationDbContext context, IRealtimeNotifier realtime)
{
    public async Task<ApiResponseDto<OrderDto>> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        // ... create order ...
        await realtime.SendToUserAsync(order.CustomerId, "OrderCreated", new { order.Id, order.Total }, ct);
        return ApiResponseDto<OrderDto>.Success(dto);
    }
}
```

`eventName` is the client-side handler name; `payload` is serialized to JSON.

## The hub

An authenticated `NotificationsHub` is mapped at **`/hubs/notifications`**. Connections are mapped to
users by the `NameIdentifier` claim, so `SendToUserAsync` reaches every device a user is signed in on.
Clients can also join named groups:

```csharp
public Task JoinGroup(string group);   // hub method
public Task LeaveGroup(string group);
```

## Connecting from a browser

Because browsers can't set an `Authorization` header on a WebSocket, the JWT is passed via the
`access_token` query string (handled automatically by the SignalR JS client):

```ts
import { HubConnectionBuilder } from "@microsoft/signalr";

const connection = new HubConnectionBuilder()
  .withUrl("/hubs/notifications", { accessTokenFactory: () => myJwt })
  .withAutomaticReconnect()
  .build();

connection.on("OrderCreated", (order) => console.log("New order", order));
await connection.start();
await connection.invoke("JoinGroup", "orders");
```

## Scaling out

For multiple API instances, add a SignalR backplane (e.g. Redis) so messages reach clients connected
to any instance — swap `AddSignalR()` for `AddSignalR().AddStackExchangeRedis(...)` in
`RealtimeServiceExtensions`.

## Related

- [Authentication Overview](/docs/authentication/overview)
- [Mail](/docs/mail/overview)
