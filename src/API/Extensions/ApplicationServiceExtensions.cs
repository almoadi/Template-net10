using Template_net10.Application;
using Template_net10.Infrastructure;

namespace Template_net10.API.Extensions;

public static class ApplicationServiceExtensions
{
    /// <summary>Registers Application + Infrastructure layers.</summary>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);
        return services;
    }
}
