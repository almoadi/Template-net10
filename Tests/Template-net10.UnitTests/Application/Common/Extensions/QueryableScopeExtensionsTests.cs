using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Template_net10.Application.Common.Extensions;
using Template_net10.Domain.Auth.Entities;
using Template_net10.UnitTests.Common;

namespace Template_net10.UnitTests.Application.Common.Extensions;

[TestFixture]
public sealed class QueryableScopeExtensionsTests
{
    [Test]
    public async Task Search_filters_by_specified_columns()
    {
        await using var context = TestDbContextFactory.Create();
        context.Users.Add(User.Create("Alice", "اسم", "alice@example.com", "+966500000001", "hash"));
        context.Users.Add(User.Create("Bob", "اسم", "bob@example.com", "+966500000002", "hash"));
        await context.SaveChangesAsync();

        var byEmail = await context.Users.AsNoTracking()
            .Search("alice@", u => u.Email)
            .ToListAsync();

        byEmail.Should().ContainSingle(u => u.Email == "alice@example.com");
    }

    [Test]
    public async Task Search_with_multiple_columns_matches_any()
    {
        await using var context = TestDbContextFactory.Create();
        context.Roles.Add(Role.Create("Admin", "مدير"));
        context.Roles.Add(Role.Create("User", "مستخدم"));
        await context.SaveChangesAsync();

        var results = await context.Roles.AsNoTracking()
            .Search("مد", r => r.NameEn, r => r.NameAr)
            .ToListAsync();

        results.Should().ContainSingle(r => r.NameAr == "مدير");
    }

    [Test]
    public async Task Search_with_null_or_whitespace_returns_all()
    {
        await using var context = TestDbContextFactory.Create();
        context.Users.Add(User.Create("One", "واحد", "one@example.com", "+966500000005", "hash"));
        context.Users.Add(User.Create("Two", "اثنان", "two@example.com", "+966500000006", "hash"));
        await context.SaveChangesAsync();

        var results = await context.Users.AsNoTracking().Search("   ", u => u.NameEn).ToListAsync();

        results.Should().HaveCount(2);
    }

    [Test]
    public async Task OrderById_sorts_by_surrogate_key()
    {
        await using var context = TestDbContextFactory.Create();
        context.Users.Add(User.Create("Second", "ثاني", "second@example.com", "+966500000010", "hash"));
        context.Users.Add(User.Create("First", "أول", "first@example.com", "+966500000011", "hash"));
        await context.SaveChangesAsync();

        var ids = await context.Users.AsNoTracking().OrderById().Select(u => u.Id).ToListAsync();

        ids.Should().BeInAscendingOrder();
    }

    [Test]
    public async Task ActiveOnly_excludes_inactive_entities()
    {
        await using var context = TestDbContextFactory.Create();
        var active = User.Create("Active", "نشط", "active@example.com", "+966500000003", "hash");
        var inactive = User.Create("Inactive", "غير", "inactive@example.com", "+966500000004", "hash");
        inactive.Deactivate();
        context.Users.AddRange(active, inactive);
        await context.SaveChangesAsync();

        var results = await context.Users.AsNoTracking().ActiveOnly().ToListAsync();

        results.Should().ContainSingle(u => u.Email == "active@example.com");
    }

    [Test]
    public async Task WhereEquals_filters_by_property_value()
    {
        await using var context = TestDbContextFactory.Create();
        context.Users.Add(User.Create("Match", "اسم", "match@example.com", "+966500000020", "hash"));
        context.Users.Add(User.Create("Other", "اسم", "other@example.com", "+966500000021", "hash"));
        await context.SaveChangesAsync();

        var results = await context.Users.AsNoTracking()
            .WhereEquals(u => u.Email, "match@example.com")
            .ToListAsync();

        results.Should().ContainSingle(u => u.NameEn == "Match");
    }

    [Test]
    public async Task WhereIn_filters_by_set_of_values()
    {
        await using var context = TestDbContextFactory.Create();
        context.Roles.Add(Role.Create("Admin", "مدير"));
        context.Roles.Add(Role.Create("User", "مستخدم"));
        context.Roles.Add(Role.Create("Guest", "ضيف"));
        await context.SaveChangesAsync();

        var results = await context.Roles.AsNoTracking()
            .WhereIn(r => r.NameEn, ["Admin", "Guest"])
            .OrderById()
            .Select(r => r.NameEn)
            .ToListAsync();

        results.Should().Equal("Admin", "Guest");
    }

    [Test]
    public async Task WhereId_returns_single_entity()
    {
        await using var context = TestDbContextFactory.Create();
        var user = User.Create("Target", "هدف", "target@example.com", "+966500000030", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var found = await context.Users.AsNoTracking().WhereId(user.Id).SingleAsync();

        found.Email.Should().Be("target@example.com");
    }

    [Test]
    public async Task CreatedAfter_filters_by_created_timestamp()
    {
        await using var context = TestDbContextFactory.Create();
        var cutoff = DateTime.UtcNow.AddMinutes(-5);
        context.Users.Add(User.Create("Old", "قديم", "old@example.com", "+966500000040", "hash"));
        await context.SaveChangesAsync();

        var results = await context.Users.AsNoTracking().CreatedAfter(cutoff).ToListAsync();

        results.Should().NotBeEmpty();
    }
}
