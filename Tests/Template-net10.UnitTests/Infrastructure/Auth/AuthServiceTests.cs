using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Infrastructure.Services;
using Template_net10.UnitTests.Common;

namespace Template_net10.UnitTests.Infrastructure.Auth;

[TestFixture]
public sealed class AuthServiceTests
{
    [Test]
    public async Task Attempt_with_valid_credentials_issues_token_with_roles_and_permissions()
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

        string[]? capturedRoles = null;
        string[]? capturedPermissions = null;
        var tokenService = new Mock<IJwtTokenService>();
        tokenService
            .Setup(t => t.GenerateToken(It.IsAny<int>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Callback<int, IEnumerable<string>, IEnumerable<string>>((_, roles, perms) =>
            {
                capturedRoles = roles.ToArray();
                capturedPermissions = perms.ToArray();
            })
            .Returns(("token", DateTime.UtcNow.AddHours(1)));

        var auth = new AuthService(
            new Mock<IHttpContextAccessor>().Object, context, hasher.Object, tokenService.Object);

        var result = await auth.Attempt("admin@example.com", "secret", CancellationToken.None);

        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("token");
        capturedRoles.Should().BeEquivalentTo("Admin");
        capturedPermissions.Should().BeEquivalentTo("users.read", "users.write");
    }

    [Test]
    public async Task Attempt_with_wrong_password_returns_null()
    {
        await using var context = TestDbContextFactory.Create();
        context.Users.Add(User.Create("Admin", "مدير", "admin@example.com", "+966500000000", "stored-hash"));
        await context.SaveChangesAsync();

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var auth = new AuthService(
            new Mock<IHttpContextAccessor>().Object, context, hasher.Object, new Mock<IJwtTokenService>().Object);

        var result = await auth.Attempt("admin@example.com", "wrong", CancellationToken.None);

        result.Should().BeNull();
    }

    [Test]
    public async Task Validate_returns_true_only_for_correct_credentials()
    {
        await using var context = TestDbContextFactory.Create();
        context.Users.Add(User.Create("Admin", "مدير", "admin@example.com", "+966500000000", "stored-hash"));
        await context.SaveChangesAsync();

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.Verify("stored-hash", "secret")).Returns(true);

        var auth = new AuthService(
            new Mock<IHttpContextAccessor>().Object, context, hasher.Object, new Mock<IJwtTokenService>().Object);

        (await auth.Validate("admin@example.com", "secret", CancellationToken.None)).Should().BeTrue();
        (await auth.Validate("admin@example.com", "nope", CancellationToken.None)).Should().BeFalse();
    }
}
