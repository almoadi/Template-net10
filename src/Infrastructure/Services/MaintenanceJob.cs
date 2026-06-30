using Microsoft.Extensions.Logging;
using Template_net10.Application.Abstractions.Jobs;

namespace Template_net10.Infrastructure.Services;

/// <summary>Sample recurring job — logs a heartbeat. Replace with real periodic maintenance work.</summary>
public sealed class MaintenanceJob : IMaintenanceJob
{
    private readonly ILogger<MaintenanceJob> _logger;

    public MaintenanceJob(ILogger<MaintenanceJob> logger) => _logger = logger;

    public Task HeartbeatAsync()
    {
        _logger.LogInformation("Maintenance heartbeat at {UtcNow:o}", DateTime.UtcNow);
        return Task.CompletedTask;
    }
}
