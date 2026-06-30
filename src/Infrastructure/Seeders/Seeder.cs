using Microsoft.Extensions.DependencyInjection;

namespace Template_net10.Infrastructure.Seeders;

/// <summary>
/// Laravel-style base seeder. Subclasses override <see cref="RunAsync"/> and may invoke other
/// seeders through <see cref="Call{TSeeder}"/> / <see cref="Call(System.Type[])"/> — the C#
/// equivalent of Laravel's <c>$this-&gt;call(...)</c>.
/// </summary>
public abstract class Seeder
{
    /// <summary>Scoped service provider used to resolve dependencies for nested seeders.</summary>
    protected IServiceProvider Services { get; private set; } = null!;

    /// <summary>Shared EF Core context for the current seeding run.</summary>
    protected ApplicationDbContext Context { get; private set; } = null!;

    /// <summary>Cancellation token flowed through the whole seeding run.</summary>
    protected CancellationToken CancellationToken { get; private set; }

    /// <summary>Wires the ambient run state. Called by the runner and by <see cref="Call(System.Type[])"/>.</summary>
    internal void Initialize(IServiceProvider services, ApplicationDbContext context, CancellationToken cancellationToken)
    {
        Services = services;
        Context = context;
        CancellationToken = cancellationToken;
    }

    /// <summary>Seeding logic for this seeder.</summary>
    public abstract Task RunAsync();

    /// <summary>Run a single nested seeder, resolving its constructor dependencies from DI.</summary>
    protected Task Call<TSeeder>()
        where TSeeder : Seeder
        => RunSeederAsync(typeof(TSeeder));

    /// <summary>Run a sequence of nested seeders in order — mirrors Laravel's <c>$this-&gt;call([...])</c>.</summary>
    protected async Task Call(params Type[] seeders)
    {
        foreach (var seeder in seeders)
        {
            await RunSeederAsync(seeder);
        }
    }

    private async Task RunSeederAsync(Type seederType)
    {
        if (!typeof(Seeder).IsAssignableFrom(seederType))
        {
            throw new ArgumentException($"{seederType.Name} is not a {nameof(Seeder)}.", nameof(seederType));
        }

        var seeder = (Seeder)ActivatorUtilities.CreateInstance(Services, seederType);
        seeder.Initialize(Services, Context, CancellationToken);
        await seeder.RunAsync();
    }
}
