using Microsoft.Extensions.DependencyInjection;
using Template_net10.Infrastructure;
using Template_net10.Infrastructure.Middleware;
using Template_net10.Infrastructure.Seeders;

namespace Template_net10.API.Extensions;

public static class MiddlewarePipelineExtensions
{
    /// <summary>Composes the HTTP request pipeline in the correct order.</summary>
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors(CorsServiceExtensions.ClientCorsPolicy);
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapDefaultEndpoints();

        return app;
    }

    /// <summary>Applies migrations and seeds baseline data; logs and continues if the DB is unreachable.</summary>
    public static async Task MigrateAndSeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;
        var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

        try
        {
            var initializer = provider.GetRequiredService<ApplicationDbInitializer>();
            await initializer.MigrateAsync();
            await provider.SeedDatabaseAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Database migrate/seed skipped — the database is unavailable.");
        }
    }
}
