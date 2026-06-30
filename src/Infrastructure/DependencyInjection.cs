using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Infrastructure.Authorization;
using Template_net10.Infrastructure.Middleware;
using Template_net10.Infrastructure.Options;
using Template_net10.Infrastructure.Services;

namespace Template_net10.Infrastructure;

/// <summary>Registers Infrastructure: EF Core, services (via Scrutor), caching, authorization, health checks.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbInitializer>();

        services.AddHttpContextAccessor();

        // Scan Infrastructure.Services and bind each implementation to its interface(s).
        services.Scan(scan => scan
            .FromAssemblyOf<LocalizationService>()
            .AddClasses(c => c.InNamespaceOf<LocalizationService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        // Permission-based authorization.
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddAuthorization();

        // Distributed cache: Redis when configured, otherwise in-memory.
        var redis = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redis))
        {
            services.AddStackExchangeRedisCache(o => o.Configuration = redis);
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddScoped<ExceptionHandlingMiddleware>();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        return services;
    }
}
