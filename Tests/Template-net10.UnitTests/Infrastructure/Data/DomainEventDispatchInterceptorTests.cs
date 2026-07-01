using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Template_net10.Application.Auth.Users;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Domain.Auth.Events;
using Template_net10.UnitTests.Common;

namespace Template_net10.UnitTests.Infrastructure.Data;

[TestFixture]
public sealed class DomainEventDispatchInterceptorTests
{
    [Test]
    public async Task Dispatches_UserCreatedDomainEvent_after_user_insert()
    {
        UserCreatedDomainEvent? captured = null;
        await using var context = TestDbContextFactory.Create(
            new CapturingDomainEventDispatcher(e => captured = e as UserCreatedDomainEvent));

        var user = User.Create("Test User", "اسم", "test@example.com", "+966500000099", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        captured.Should().NotBeNull();
        captured!.UserId.Should().Be(user.Id);
        captured.Email.Should().Be("test@example.com");
    }
}
