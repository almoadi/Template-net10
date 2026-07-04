using Asp.Versioning.ApiExplorer;
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
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // One Swagger UI dropdown entry per discovered API version (newest first).
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions.Reverse())
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }

                options.EnablePersistAuthorization();
            });
        }

        app.UseRequestLocalization();
        app.UseHttpsRedirection();
        app.UseCors(CorsServiceExtensions.ClientCorsPolicy);
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<IdempotencyMiddleware>();

        app.UseAppHangfire();

        app.MapControllers();
        app.MapRealtime();
        app.MapDefaultEndpoints();

        return app;
    }

    /// <summary>Applies migrations and seeds baseline data; logs and continues so the app still starts.</summary>
    public static async Task MigrateAndSeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;
        var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

        try
        {
            // MigrateAsync creates the database if it does not exist, then applies pending migrations.
            var initializer = provider.GetRequiredService<ApplicationDbInitializer>();
            await initializer.MigrateAsync();
            await provider.SeedDatabaseAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Database migrate/seed failed. Check the connection string in config/database.json and that " +
                "SQL Server is reachable. If the dev schema is out of sync, reset it: " +
                "dotnet ef database drop -f --project src/Infrastructure --startup-project src/API && " +
                "dotnet ef database update --project src/Infrastructure --startup-project src/API");
        }
    }
}
