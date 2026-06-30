using Microsoft.EntityFrameworkCore;
using Template_net10.Domain.Auth.Constants;
using Template_net10.Domain.Auth.Entities;

namespace Template_net10.Infrastructure.Seeders.Auth;

/// <summary>
/// Seeds the permission catalog from <see cref="PermissionRegistry"/>. Idempotent and additive:
/// inserts any code that does not yet exist, so adding a new permission is just a re-seed.
/// </summary>
public sealed class PermissionSeeder : Seeder
{
    public override async Task RunAsync()
    {
        var existingCodes = await Context.Permissions
            .Select(p => p.Code)
            .ToListAsync(CancellationToken);

        var missing = PermissionRegistry.All
            .Where(def => !existingCodes.Contains(def.Code))
            .Select(def => Permission.Create(def.Code, def.NameEn, def.NameAr))
            .ToList();

        if (missing.Count == 0)
        {
            return;
        }

        Context.Permissions.AddRange(missing);
        await Context.SaveChangesAsync(CancellationToken);
    }
}
