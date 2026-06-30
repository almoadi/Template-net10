using FluentAssertions;
using NUnit.Framework;
using Template_net10.Application.Auth.Users.Queries.SearchUsers;
using Template_net10.Domain.Auth.Entities;
using Template_net10.UnitTests.Common;

namespace Template_net10.UnitTests.Application.Auth.Users;

[TestFixture]
public sealed class SearchUsersQueryHandlerTests
{
    [Test]
    public async Task Returns_paged_result_with_total_count_and_applied_limit()
    {
        await using var context = TestDbContextFactory.Create();
        for (var i = 1; i <= 25; i++)
        {
            context.Users.Add(User.Create($"User{i:00}", "اسم", $"user{i:00}@example.com", $"+96650000{i:0000}", "hash"));
        }

        await context.SaveChangesAsync();

        var handler = new SearchUsersQueryHandler(context);
        var result = await handler.Handle(new SearchUsersQuery { Limit = 10, Offset = 0 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Count.Should().Be(10);
        result.MetaData.ResultSet.Count.Should().Be(25);
        result.MetaData.ResultSet.Limit.Should().Be(10);
        result.MetaData.ResultSet.Offset.Should().Be(0);
    }

    [Test]
    public async Task Filters_by_search_term()
    {
        await using var context = TestDbContextFactory.Create();
        context.Users.Add(User.Create("Alice", "اسم", "alice@example.com", "+966500000001", "hash"));
        context.Users.Add(User.Create("Bob", "اسم", "bob@example.com", "+966500000002", "hash"));
        await context.SaveChangesAsync();

        var handler = new SearchUsersQueryHandler(context);
        var result = await handler.Handle(new SearchUsersQuery { Search = "Ali" }, CancellationToken.None);

        result.Data!.Should().ContainSingle(u => u.NameEn == "Alice");
    }
}
