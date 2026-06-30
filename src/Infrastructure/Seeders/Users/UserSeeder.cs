using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Domain.Auth.Constants;
using Template_net10.Domain.Auth.Entities;

namespace Template_net10.Infrastructure.Seeders.Users;

/// <summary>
/// Seeds the default administrator and assigns the Admin role. Idempotent: skips when a user
/// with the admin email already exists. Runs after the role/permission seeders.
/// </summary>
public sealed class UserSeeder : Seeder
{
    private const string AdminEmail = "admin@template-net10.local";

    private readonly IPasswordHasher _passwordHasher;

    public UserSeeder(IPasswordHasher passwordHasher) => _passwordHasher = passwordHasher;

    public override async Task RunAsync()
    {
        if (await Context.Users.AnyAsync(u => u.Email == AdminEmail, CancellationToken))
        {
            return;
        }

        var adminRole = await Context.Roles
            .FirstOrDefaultAsync(r => r.NameEn == AuthRoles.Admin, CancellationToken);

        var admin = User.Create(
            nameEn: "Administrator",
            nameAr: "مدير النظام",
            email: AdminEmail,
            phone: "+966500000000",
            passwordHash: _passwordHasher.Hash("ChangeMe!123"));

        if (adminRole is not null)
        {
            admin.AssignRole(adminRole);
        }

        Context.Users.Add(admin);
        await Context.SaveChangesAsync(CancellationToken);
    }
}
