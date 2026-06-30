using Hangfire;
using Microsoft.Extensions.Options;
using Template_net10.Application.Abstractions.Jobs;
using Template_net10.Infrastructure.Authorization;
using Template_net10.Infrastructure.Options;

namespace Template_net10.API.Extensions;

public static class HangfireServiceExtensions
{
    /// <summary>
    /// Mounts the Hangfire dashboard (when enabled in config/queue.json) and registers the
    /// application's recurring jobs. Call after authentication/authorization middleware.
    /// </summary>
    public static WebApplication UseAppHangfire(this WebApplication app)
    {
        var queue = app.Services.GetRequiredService<IOptions<QueueOptions>>().Value;

        if (queue.DashboardEnabled)
        {
            app.UseHangfireDashboard(queue.DashboardPath, new DashboardOptions
            {
                Authorization = [new HangfireDashboardAuthorizationFilter(app.Environment.IsDevelopment())],
            });
        }

        RegisterRecurringJobs(app.Services);
        return app;
    }

    private static void RegisterRecurringJobs(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var scheduler = scope.ServiceProvider.GetRequiredService<IJobScheduler>();

        // Sample daily recurring job (03:00 UTC). Add real periodic work the same way.
        scheduler.AddOrUpdateRecurring<IMaintenanceJob>(
            "maintenance-heartbeat", job => job.HeartbeatAsync(), cronExpression: "0 3 * * *");
    }
}
