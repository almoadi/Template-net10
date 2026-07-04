using Template_net10.Infrastructure.Services.Realtime;

namespace Template_net10.API.Extensions;

public static class RealtimeServiceExtensions
{
    /// <summary>Path the SignalR notifications hub is mapped to.</summary>
    public const string HubPath = "/hubs/notifications";

    /// <summary>Registers SignalR (WebSocket real-time) services.</summary>
    public static IServiceCollection AddRealtime(this IServiceCollection services)
    {
        services.AddSignalR();
        return services;
    }

    /// <summary>Maps the notifications hub endpoint.</summary>
    public static WebApplication MapRealtime(this WebApplication app)
    {
        app.MapHub<NotificationsHub>(HubPath);
        return app;
    }
}
