using Microsoft.EntityFrameworkCore;

namespace Template_net10.Infrastructure;

/// <summary>
/// Applies pending EF Core migrations on startup. Data seeding is handled separately by the
/// <see cref="Seeders.DatabaseSeeder"/> system.
/// </summary>
public sealed class ApplicationDbInitializer
{
    private readonly ApplicationDbContext _context;

    public ApplicationDbInitializer(ApplicationDbContext context) => _context = context;

    public async Task MigrateAsync(CancellationToken ct = default)
    {
        if (_context.Database.IsRelational())
        {
            await _context.Database.MigrateAsync(ct);
        }
    }
}
