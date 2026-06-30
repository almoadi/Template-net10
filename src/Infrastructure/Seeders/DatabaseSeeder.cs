using Template_net10.Infrastructure.Seeders.Auth;
using Template_net10.Infrastructure.Seeders.Users;

namespace Template_net10.Infrastructure.Seeders;

/// <summary>
/// Root seeder — the entry point invoked by the runner. Register every other seeder here,
/// just like Laravel's <c>DatabaseSeeder</c>. Order matters: permissions, then roles, then users.
/// </summary>
public sealed class DatabaseSeeder : Seeder
{
    public override async Task RunAsync()
    {
        await Call(
            typeof(PermissionSeeder),
            typeof(RoleSeeder),
            typeof(UserSeeder));
    }
}
