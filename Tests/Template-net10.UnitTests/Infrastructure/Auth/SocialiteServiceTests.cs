using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Template_net10.Application.Abstractions.Auth.Social;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Domain.Auth.Enums;
using Template_net10.Domain.Common.Exceptions;
using Template_net10.Infrastructure;
using Template_net10.Infrastructure.Options;
using Template_net10.Infrastructure.Services.Auth;
using Template_net10.Infrastructure.Services.Auth.Social;
using Template_net10.UnitTests.Common;

namespace Template_net10.UnitTests.Infrastructure.Auth;

[TestFixture]
public sealed class SocialiteServiceTests
{
    private static readonly SocialUser GoogleUser = new()
    {
        Provider = SocialProvider.Google,
        ProviderUserId = "google-123",
        Email = "ada@example.com",
        Name = "Ada Lovelace",
        AvatarUrl = "https://example.com/a.png",
    };

    [Test]
    public async Task LoginWithToken_provisions_and_links_a_new_user()
    {
        await using var context = TestDbContextFactory.Create();
        var (service, tokenIssuer) = BuildService(context, googleEnabled: true);

        var result = await service.LoginWithTokenAsync(SocialProvider.Google, "provider-token", CancellationToken.None);

        result.AccessToken.Should().Be("issued-jwt");

        var user = context.Users.Single();
        user.Email.Should().Be("ada@example.com");
        user.PasswordHash.Should().BeEmpty();

        var account = context.SocialAccounts.Single();
        account.Provider.Should().Be(SocialProvider.Google);
        account.ProviderUserId.Should().Be("google-123");
        account.UserId.Should().Be(user.Id);

        tokenIssuer.Verify(t => t.IssueAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task LoginWithToken_links_to_an_existing_user_matched_by_email()
    {
        await using var context = TestDbContextFactory.Create();
        context.Users.Add(User.Create("Ada", "Ada", "ada@example.com", "+966500000000", "hash"));
        await context.SaveChangesAsync();

        var (service, _) = BuildService(context, googleEnabled: true);

        await service.LoginWithTokenAsync(SocialProvider.Google, "provider-token", CancellationToken.None);

        context.Users.Should().HaveCount(1, "the existing user is reused, not duplicated");
        context.SocialAccounts.Single().UserId.Should().Be(context.Users.Single().Id);
    }

    [Test]
    public async Task LoginWithToken_reuses_the_existing_social_account_link()
    {
        await using var context = TestDbContextFactory.Create();
        var (service, _) = BuildService(context, googleEnabled: true);

        await service.LoginWithTokenAsync(SocialProvider.Google, "t1", CancellationToken.None);
        await service.LoginWithTokenAsync(SocialProvider.Google, "t2", CancellationToken.None);

        context.Users.Should().HaveCount(1);
        context.SocialAccounts.Should().HaveCount(1);
    }

    [Test]
    public async Task LoginWithToken_throws_when_provider_is_disabled()
    {
        await using var context = TestDbContextFactory.Create();
        var (service, _) = BuildService(context, googleEnabled: false);

        var act = () => service.LoginWithTokenAsync(SocialProvider.Google, "provider-token", CancellationToken.None);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    private static (SocialiteService Service, Mock<ITokenIssuer> TokenIssuer) BuildService(
        ApplicationDbContext context, bool googleEnabled)
    {
        var driver = new Mock<ISocialProviderDriver>();
        driver.SetupGet(d => d.Provider).Returns(SocialProvider.Google);
        driver.Setup(d => d.GetUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GoogleUser);

        var tokenIssuer = new Mock<ITokenIssuer>();
        tokenIssuer.Setup(t => t.IssueAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthTokenDto
            {
                AccessToken = "issued-jwt",
                ExpiresAtUtc = DateTime.UtcNow.AddHours(1),
                RefreshToken = "refresh",
                RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(30),
            });

        var localization = new Mock<ILocalizationService>();
        localization.Setup(l => l.GetMessage(It.IsAny<Resource>())).Returns("message");

        var options = Options.Create(new SocialiteOptions
        {
            Google = new SocialProviderOptions { Enabled = googleEnabled },
        });

        var service = new SocialiteService(
            [driver.Object], context, tokenIssuer.Object, localization.Object, options);

        return (service, tokenIssuer);
    }
}
