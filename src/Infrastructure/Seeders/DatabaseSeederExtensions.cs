using Microsoft.Extensions.DependencyInjection;

namespace Template_net10.Infrastructure.Seeders;

/// <summary>Host-facing entry point that runs the <see cref="DatabaseSeeder"/> — the C# equivalent of <c>php artisan db:seed</c>.</summary>
public static class DatabaseSeederExtensions
{
    /// <summary>
    /// Creates a DI scope, resolves the <see cref="ApplicationDbContext"/>, then runs the root
    /// <see cref="DatabaseSeeder"/>. Call from <c>Program.cs</c>, e.g.
    /// <c>await app.Services.SeedDatabaseAsync();</c>.
    /// </summary>
    public static async Task SeedDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;
        var context = provider.GetRequiredService<ApplicationDbContext>();

        var seeder = ActivatorUtilities.CreateInstance<DatabaseSeeder>(provider);
        seeder.Initialize(provider, context, cancellationToken);
        await seeder.RunAsync();
    }
}
