using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Abstractions.Jobs;
using Template_net10.Application.Abstractions.Messaging;
using Template_net10.Application.Auth.Users.Events;
using Template_net10.Infrastructure.Authorization;
using Template_net10.Infrastructure.Data;
using Template_net10.Infrastructure.Jobs;
using Template_net10.Infrastructure.Middleware;
using Template_net10.Infrastructure.Options;
using Template_net10.Infrastructure.Services;

namespace Template_net10.Infrastructure;

/// <summary>Registers Infrastructure: EF Core, services (via Scrutor), caching, mail, authorization, health checks.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Bind the Laravel-style config sections to strongly-typed options.
        services.Configure<AppOptions>(configuration.GetSection(AppOptions.SectionName));
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.SectionName));
        services.Configure<MailOptions>(configuration.GetSection(MailOptions.SectionName));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        var database = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();
        var cache = configuration.GetSection(CacheOptions.SectionName).Get<CacheOptions>() ?? new CacheOptions();

        // Database connection string falls back to ConnectionStrings:DefaultConnection.
        var connectionString = !string.IsNullOrWhiteSpace(database.ConnectionString)
            ? database.ConnectionString
            : configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, sql =>
            {
                sql.CommandTimeout(database.CommandTimeoutSeconds);
                sql.EnableRetryOnFailure(database.MaxRetryCount);
            });

            options.EnableDetailedErrors(database.EnableDetailedErrors);
            options.EnableSensitiveDataLogging(database.EnableSensitiveDataLogging);
            options.AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>());
        });

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<DomainEventDispatchInterceptor>();

        services.Scan(scan => scan
            .FromAssemblyOf<UserCreatedDomainEventHandler>()
            .AddClasses(c => c.Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbInitializer>();

        services.AddHttpContextAccessor();

        // Scan Infrastructure.Services and bind each implementation to its interface(s).
        services.Scan(scan => scan
            .FromAssemblyOf<LocalizationService>()
            .AddClasses(c => c.InNamespaceOf<LocalizationService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Permission-based authorization.
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddAuthorization();

        // Cache: driver selected in config/cache.json (Memory or Redis).
        if (cache.Driver == CacheDriver.Redis && !string.IsNullOrWhiteSpace(cache.RedisConnection))
        {
            services.AddStackExchangeRedisCache(o =>
            {
                o.Configuration = cache.RedisConnection;
                o.InstanceName = cache.InstanceName;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddScoped<ExceptionHandlingMiddleware>();

        // Background jobs / queue: Hangfire backed by SQL Server (config/queue.json).
        services.Configure<QueueOptions>(configuration.GetSection(QueueOptions.SectionName));
        var queue = configuration.GetSection(QueueOptions.SectionName).Get<QueueOptions>() ?? new QueueOptions();

        services.AddHangfire(hangfire => hangfire
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                SchemaName = queue.SchemaName,
                PrepareSchemaIfNecessary = true,
                QueuePollInterval = TimeSpan.FromSeconds(15),
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true,
            }));

        services.AddHangfireServer(options =>
        {
            options.Queues = queue.Queues is { Length: > 0 } ? queue.Queues : ["default"];
            if (queue.WorkerCount > 0)
            {
                options.WorkerCount = queue.WorkerCount;
            }
        });

        services.AddScoped<IJobScheduler, HangfireJobScheduler>();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        return services;
    }
}
