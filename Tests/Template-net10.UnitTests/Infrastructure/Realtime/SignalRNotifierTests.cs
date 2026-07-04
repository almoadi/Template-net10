using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using Template_net10.Infrastructure.Services.Realtime;

namespace Template_net10.UnitTests.Infrastructure.Realtime;

[TestFixture]
public sealed class SignalRNotifierTests
{
    [Test]
    public async Task SendToAllAsync_sends_event_to_all_clients()
    {
        var proxy = new Mock<IClientProxy>();
        var clients = new Mock<IHubClients>();
        clients.Setup(c => c.All).Returns(proxy.Object);

        var hubContext = new Mock<IHubContext<NotificationsHub>>();
        hubContext.Setup(h => h.Clients).Returns(clients.Object);

        var notifier = new SignalRNotifier(hubContext.Object);
        var payload = new { Message = "hello" };

        await notifier.SendToAllAsync("notify", payload);

        proxy.Verify(
            p => p.SendCoreAsync(
                "notify",
                It.Is<object[]>(args => args.Length == 1 && ReferenceEquals(args[0], payload)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task SendToGroupAsync_sends_event_to_named_group()
    {
        var proxy = new Mock<IClientProxy>();
        var clients = new Mock<IHubClients>();
        clients.Setup(c => c.Group("room-1")).Returns(proxy.Object);

        var hubContext = new Mock<IHubContext<NotificationsHub>>();
        hubContext.Setup(h => h.Clients).Returns(clients.Object);

        var notifier = new SignalRNotifier(hubContext.Object);

        await notifier.SendToGroupAsync("room-1", "joined", new { UserId = 7 });

        proxy.Verify(
            p => p.SendCoreAsync("joined", It.IsAny<object[]>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
