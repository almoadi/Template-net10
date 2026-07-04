using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Template_net10.Application.Abstractions.Caching;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Abstractions.Notifications;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Domain.Common.Exceptions;
using Template_net10.Infrastructure.Options;
using Template_net10.Infrastructure.Services.Auth;
using Template_net10.UnitTests.Common;

namespace Template_net10.UnitTests.Infrastructure.Auth;

[TestFixture]
public sealed class AccountServiceTests
{
    private const string Password = "Secret123!";

    private static (AccountService Service, CapturingEmailSender Email, ApplicationDbContextAlias Db) Build(
        AuthOptions options, Mock<ITokenIssuer>? issuer = null)
    {
        var context = TestDbContextFactory.Create();
        var hasher = new PasswordHasherService();
        var email = new CapturingEmailSender();
        issuer ??= new Mock<ITokenIssuer>();

        var service = new AccountService(
            context,
            hasher,
            email,
            new StubTemplateRenderer(),
            new FakeCache(),
            new StubLocalization(),
            issuer.Object,
            Options.Create(options),
            Options.Create(new AppOptions { Name = "Test" }));

        return (service, email, new ApplicationDbContextAlias(context, hasher));
    }

    private static async Task<User> SeedUserAsync(ApplicationDbContextAlias db, bool verified = false)
    {
        var user = User.Create("Test", "اختبار", "user@example.com", "+966500000001", db.Hasher.Hash(Password));
        if (verified)
        {
            user.MarkEmailVerified();
        }

        db.Context.Users.Add(user);
        await db.Context.SaveChangesAsync();
        return user;
    }

    [Test]
    public async Task EnsureLoginAllowed_throws_when_email_unverified_and_required()
    {
        var (service, _, db) = Build(new AuthOptions { RequireEmailVerification = true });
        var user = await SeedUserAsync(db);

        var act = async () => await service.EnsureLoginAllowedAsync(user.Email, Password);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Test]
    public async Task EnsureLoginAllowed_is_silent_for_verified_user_without_two_factor()
    {
        var (service, _, db) = Build(new AuthOptions { RequireEmailVerification = true });
        var user = await SeedUserAsync(db, verified: true);

        var act = async () => await service.EnsureLoginAllowedAsync(user.Email, Password);

        await act.Should().NotThrowAsync();
    }

    [Test]
    public async Task Email_verification_round_trips_and_is_single_use()
    {
        var (service, email, db) = Build(new AuthOptions());
        var user = await SeedUserAsync(db);

        await service.SendEmailVerificationAsync(user.Email);
        var token = ExtractCode(email.Sent.Single());

        (await service.VerifyEmailAsync(user.Email, token)).Should().BeTrue();
        (await service.VerifyEmailAsync(user.Email, token)).Should().BeFalse(); // already consumed
    }

    [Test]
    public async Task Password_reset_round_trips_and_changes_the_hash()
    {
        var (service, email, db) = Build(new AuthOptions());
        var user = await SeedUserAsync(db);

        await service.SendPasswordResetAsync(user.Email);
        var token = ExtractCode(email.Sent.Single());

        (await service.ResetPasswordAsync(user.Email, token, "NewPass456!")).Should().BeTrue();

        var updated = db.Context.Users.Single(u => u.Id == user.Id);
        db.Hasher.Verify(updated.PasswordHash, "NewPass456!").Should().BeTrue();
    }

    [Test]
    public async Task Two_factor_login_sends_code_then_issues_token()
    {
        var issuer = new Mock<ITokenIssuer>();
        issuer.Setup(i => i.IssueAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthTokenDto
            {
                AccessToken = "access",
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
                RefreshToken = "refresh",
                RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(30),
            });

        var (service, email, db) = Build(new AuthOptions { TwoFactorEnabled = true }, issuer);
        var user = await SeedUserAsync(db, verified: true);

        var login = async () => await service.EnsureLoginAllowedAsync(user.Email, Password);
        await login.Should().ThrowAsync<BadRequestException>();

        var code = ExtractCode(email.Sent.Single());
        var token = await service.VerifyTwoFactorAsync(user.Email, code);

        token.Should().NotBeNull();
        token!.AccessToken.Should().Be("access");
    }

    private static string ExtractCode(EmailMessage message)
        => Regex.Match(message.HtmlBody, "<strong>(.+?)</strong>").Groups[1].Value;

    private sealed class CapturingEmailSender : IEmailSender
    {
        public List<EmailMessage> Sent { get; } = [];

        public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            Sent.Add(message);
            return Task.CompletedTask;
        }

        public Task SendBulkAsync(IReadOnlyList<EmailMessage> messages, CancellationToken cancellationToken = default)
        {
            Sent.AddRange(messages);
            return Task.CompletedTask;
        }
    }

    private sealed class StubTemplateRenderer : IEmailTemplateRenderer
    {
        public string Render(string templateName, IReadOnlyDictionary<string, string> values)
            => $"<strong>{values.GetValueOrDefault("code")}</strong>";
    }

    private sealed class FakeCache : ICacheService
    {
        private readonly Dictionary<string, object?> _store = [];

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
            => Task.FromResult(_store.TryGetValue(key, out var value) && value is T typed ? typed : default);

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            _store[key] = value;
            return Task.CompletedTask;
        }

        public async Task<T> RememberAsync<T>(
            string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            if (_store.TryGetValue(key, out var existing) && existing is T typed)
            {
                return typed;
            }

            var value = await factory(cancellationToken);
            _store[key] = value;
            return value;
        }

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
            => Task.FromResult(_store.ContainsKey(key));

        public Task ForgetAsync(string key, CancellationToken cancellationToken = default)
        {
            _store.Remove(key);
            return Task.CompletedTask;
        }
    }

    private sealed class StubLocalization : ILocalizationService
    {
        public string GetMessage(Resource resource) => resource.ToString();
    }

    // Small carrier so tests can reach the context + hasher used to build the service.
    internal sealed record ApplicationDbContextAlias(
        Template_net10.Infrastructure.ApplicationDbContext Context, PasswordHasherService Hasher);
}
