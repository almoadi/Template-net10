using Microsoft.EntityFrameworkCore;
using Template_net10.Domain.Auth.Constants;
using Template_net10.Domain.Auth.Entities;

namespace Template_net10.Infrastructure.Seeders.Auth;

/// <summary>
/// Seeds the built-in system roles. Admin receives every permission; User receives the
/// read-only permissions. Runs after <see cref="PermissionSeeder"/>.
/// </summary>
public sealed class RoleSeeder : Seeder
{
    public override async Task RunAsync()
    {
        var permissions = await Context.Permissions.ToListAsync(CancellationToken);

        await EnsureRoleAsync(AuthRoles.Admin, "مدير", permissions);

        var readOnly = permissions
            .Where(p => p.Code.EndsWith(".read", StringComparison.Ordinal))
            .ToList();

        await EnsureRoleAsync(AuthRoles.User, "مستخدم", readOnly);
    }

    private async Task EnsureRoleAsync(string nameEn, string nameAr, List<Permission> permissions)
    {
        var role = await Context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.NameEn == nameEn, CancellationToken);

        if (role is null)
        {
            role = Role.Create(nameEn, nameAr, isSystem: true);
            Context.Roles.Add(role);
        }

        role.SetPermissions(permissions);
        await Context.SaveChangesAsync(CancellationToken);
    }
}
