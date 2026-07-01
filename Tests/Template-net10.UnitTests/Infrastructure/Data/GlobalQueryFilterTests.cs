using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Template_net10.Application.Common.Extensions;
using Template_net10.UnitTests.Common;

namespace Template_net10.UnitTests.Infrastructure.Data;

[TestFixture]
public sealed class GlobalQueryFilterTests
{
    [Test]
    public async Task SoftDelete_global_scope_hides_deleted_rows()
    {
        await using var context = ScopeTestDbContextFactory.Create();
        context.Items.Add(new ScopeTestItem { Name = "Active" });
        context.Items.Add(new ScopeTestItem { Name = "Deleted" }.WithDeletedAt(DateTime.UtcNow));
        await context.SaveChangesAsync();

        var results = await context.Items.AsNoTracking().Select(x => x.Name).ToListAsync();

        results.Should().Equal("Active");
    }

    [Test]
    public async Task WithTrashed_bypasses_soft_delete_global_scope()
    {
        await using var context = ScopeTestDbContextFactory.Create();
        context.Items.Add(new ScopeTestItem { Name = "Active" });
        context.Items.Add(new ScopeTestItem { Name = "Deleted" }.WithDeletedAt(DateTime.UtcNow));
        await context.SaveChangesAsync();

        var results = await context.Items.AsNoTracking()
            .WithTrashed()
            .OrderById()
            .Select(x => x.Name)
            .ToListAsync();

        results.Should().Equal("Active", "Deleted");
    }

    [Test]
    public async Task OnlyTrashed_returns_deleted_rows_only()
    {
        await using var context = ScopeTestDbContextFactory.Create();
        context.Items.Add(new ScopeTestItem { Name = "Active" });
        context.Items.Add(new ScopeTestItem { Name = "Deleted" }.WithDeletedAt(DateTime.UtcNow));
        await context.SaveChangesAsync();

        var results = await context.Items.AsNoTracking()
            .OnlyTrashed()
            .Select(x => x.Name)
            .ToListAsync();

        results.Should().Equal("Deleted");
    }

    [Test]
    public async Task WithoutGlobalScopes_includes_soft_deleted_rows()
    {
        await using var context = ScopeTestDbContextFactory.Create();
        context.Items.Add(new ScopeTestItem { Name = "Active" });
        context.Items.Add(new ScopeTestItem { Name = "Deleted" }.WithDeletedAt(DateTime.UtcNow));
        await context.SaveChangesAsync();

        var results = await context.Items.AsNoTracking()
            .WithoutGlobalScopes()
            .OrderById()
            .Select(x => x.Name)
            .ToListAsync();

        results.Should().Equal("Active", "Deleted");
    }
}
