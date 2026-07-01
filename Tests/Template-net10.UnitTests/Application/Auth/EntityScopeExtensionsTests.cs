using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Template_net10.Application.Auth.Roles;
using Template_net10.Application.Auth.Users;
using Template_net10.Domain.Auth.Entities;
using Template_net10.UnitTests.Common;

namespace Template_net10.UnitTests.Application.Auth;

[TestFixture]
public sealed class EntityScopeExtensionsTests
{
    [Test]
    public async Task UserScopeExtensions_search_and_soft_delete_helpers_work()
    {
        await using var context = TestDbContextFactory.Create();
        var user = User.Create("Alice", "اسم", "alice@example.com", "+966500000001", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        user.SoftDelete();
        await context.SaveChangesAsync();

        var active = await context.Users.AsNoTracking().SearchUsers("alice").ToListAsync();
        var deleted = await context.Users.AsNoTracking().OnlyDeletedUsers().ToListAsync();

        active.Should().BeEmpty();
        deleted.Should().ContainSingle(u => u.Email == "alice@example.com");
    }

    [Test]
    public async Task RoleScopeExtensions_exclude_system_roles()
    {
        await using var context = TestDbContextFactory.Create();
        context.Roles.Add(Role.Create("Admin", "مدير", isSystem: true));
        context.Roles.Add(Role.Create("Editor", "محرر"));
        await context.SaveChangesAsync();

        var results = await context.Roles.AsNoTracking()
            .SearchRoles("Admin")
            .ExcludeSystemRoles()
            .ToListAsync();

        results.Should().BeEmpty();
    }
}
