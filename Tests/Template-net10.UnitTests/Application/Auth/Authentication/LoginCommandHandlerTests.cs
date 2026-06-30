using FluentAssertions;
using Moq;
using NUnit.Framework;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Auth.Authentication.Commands.Login;
using Template_net10.Domain.Auth.Entities;
using Template_net10.UnitTests.Common;

namespace Template_net10.UnitTests.Application.Auth.Authentication;

[TestFixture]
public sealed class LoginCommandHandlerTests
{
    [Test]
    public async Task Successful_login_issues_token_with_flattened_role_permissions()
    {
        await using var context = TestDbContextFactory.Create();

        var read = Permission.Create("users.read", "Read", "قراءة");
        var write = Permission.Create("users.write", "Write", "كتابة");
        context.Permissions.AddRange(read, write);

        var role = Role.Create("Admin", "مدير", isSystem: true);
        role.SetPermissions([read, write]);
        context.Roles.Add(role);

        var user = User.Create("Admin", "مدير", "admin@example.com", "+966500000000", "stored-hash");
        user.AssignRole(role);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.Verify("stored-hash", "secret")).Returns(true);

        string[]? capturedPermissions = null;
        var tokenService = new Mock<IJwtTokenService>();
        tokenService
            .Setup(t => t.GenerateToken(It.IsAny<int>(), It.IsAny<IEnumerable<string>>()))
            .Callback<int, IEnumerable<string>>((_, perms) => capturedPermissions = perms.ToArray())
            .Returns(("token", DateTime.UtcNow.AddHours(1)));

        var localization = new Mock<ILocalizationService>();

        var handler = new LoginCommandHandler(context, hasher.Object, tokenService.Object, localization.Object);

        var result = await handler.Handle(
            new LoginCommand { Email = "admin@example.com", Password = "secret" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.AccessToken.Should().Be("token");
        capturedPermissions.Should().BeEquivalentTo("users.read", "users.write");
    }
}
