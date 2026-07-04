using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Template_net10.Application.Abstractions.Account;
using Template_net10.Application.Abstractions.Caching;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Abstractions.Notifications;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Domain.Auth.Enums;
using Template_net10.Domain.Common.Exceptions;
using Template_net10.Infrastructure.Options;

namespace Template_net10.Infrastructure.Services.Auth;

/// <summary>
/// Implements the optional account-security flows (email verification, password reset, two-factor
/// login). Everything is gated by <see cref="AuthOptions"/>. One-time codes are stored only as
/// SHA-256 hashes in the distributed cache (Memory or Redis, per config/cache.json) — never in the
/// database — and expire automatically. Email bodies come from HTML templates via
/// <see cref="IEmailTemplateRenderer"/>.
/// </summary>
public sealed class AccountService : IAccountService
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateRenderer _templates;
    private readonly ICacheService _cache;
    private readonly ILocalizationService _localization;
    private readonly ITokenIssuer _tokenIssuer;
    private readonly AuthOptions _options;
    private readonly string _appName;

    public AccountService(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IEmailSender emailSender,
        IEmailTemplateRenderer templates,
        ICacheService cache,
        ILocalizationService localization,
        ITokenIssuer tokenIssuer,
        IOptions<AuthOptions> options,
        IOptions<AppOptions> appOptions)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _emailSender = emailSender;
        _templates = templates;
        _cache = cache;
        _localization = localization;
        _tokenIssuer = tokenIssuer;
        _options = options.Value;
        _appName = appOptions.Value.Name;
    }

    public bool TwoFactorRequired => _options.TwoFactorEnabled;

    public async Task EnsureLoginAllowedAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await LoadByEmailAsync(email, ct);

        // Stay silent on bad credentials — the login handler surfaces "invalid credentials" instead.
        if (user is null || !_passwordHasher.Verify(user.PasswordHash, password))
        {
            return;
        }

        if (_options.RequireEmailVerification && !user.IsEmailVerified)
        {
            throw new BadRequestException(_localization.GetMessage(Resource.EmailNotVerified));
        }

        if (_options.TwoFactorEnabled || user.TwoFactorEnabled)
        {
            var code = GenerateNumericCode(_options.OtpLength);
            await StoreOtpAsync(user.Id, OtpPurpose.TwoFactor, code, TimeSpan.FromMinutes(_options.OtpExpiryMinutes), ct);
            await SendCodeEmailAsync(
                user.Email, "two-factor", Resource.TwoFactorSubject, Resource.TwoFactorBody, code, ct);

            throw new BadRequestException(_localization.GetMessage(Resource.TwoFactorRequired));
        }
    }

    public async Task SendEmailVerificationAsync(string email, CancellationToken ct = default)
    {
        var user = await LoadByEmailAsync(email, ct);
        if (user is null || user.IsEmailVerified)
        {
            return;
        }

        var token = GenerateToken();
        await StoreOtpAsync(user.Id, OtpPurpose.EmailVerification, token, TimeSpan.FromHours(_options.EmailVerificationExpiryHours), ct);
        await SendCodeEmailAsync(
            user.Email, "email-verification", Resource.EmailVerificationSubject, Resource.EmailVerificationBody, token, ct);
    }

    public async Task<bool> VerifyEmailAsync(string email, string token, CancellationToken ct = default)
    {
        var user = await LoadTrackedByEmailAsync(email, ct);
        if (user is null || !await ConsumeOtpAsync(user.Id, OtpPurpose.EmailVerification, token, ct))
        {
            return false;
        }

        user.MarkEmailVerified();
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task SendPasswordResetAsync(string email, CancellationToken ct = default)
    {
        var user = await LoadByEmailAsync(email, ct);
        if (user is null)
        {
            return;
        }

        var token = GenerateToken();
        await StoreOtpAsync(user.Id, OtpPurpose.PasswordReset, token, TimeSpan.FromMinutes(_options.PasswordResetExpiryMinutes), ct);
        await SendCodeEmailAsync(
            user.Email, "password-reset", Resource.PasswordResetSubject, Resource.PasswordResetBody, token, ct);
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default)
    {
        var user = await LoadTrackedByEmailAsync(email, ct);
        if (user is null || !await ConsumeOtpAsync(user.Id, OtpPurpose.PasswordReset, token, ct))
        {
            return false;
        }

        user.SetPasswordHash(_passwordHasher.Hash(newPassword));

        // Revoke every active session so old credentials can no longer be used.
        var sessions = await _context.UserSessions
            .Where(s => s.UserId == user.Id && s.RevokedAt == null)
            .ToListAsync(ct);
        foreach (var session in sessions)
        {
            session.Revoke();
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<AuthTokenDto?> VerifyTwoFactorAsync(string email, string code, CancellationToken ct = default)
    {
        var user = await LoadByEmailAsync(email, ct);
        if (user is null || !await ConsumeOtpAsync(user.Id, OtpPurpose.TwoFactor, code, ct))
        {
            return null;
        }

        return await _tokenIssuer.IssueAsync(user.Id, ct);
    }

    private Task StoreOtpAsync(int userId, OtpPurpose purpose, string code, TimeSpan lifetime, CancellationToken ct)
        => _cache.SetAsync(CacheKey(userId, purpose), Hash(code), lifetime, ct);

    private async Task<bool> ConsumeOtpAsync(int userId, OtpPurpose purpose, string code, CancellationToken ct)
    {
        var key = CacheKey(userId, purpose);
        var stored = await _cache.GetAsync<string>(key, ct);
        if (stored is null || !FixedTimeEquals(stored, Hash(code)))
        {
            return false;
        }

        await _cache.ForgetAsync(key, ct); // single use
        return true;
    }

    private Task SendCodeEmailAsync(
        string email, string templateName, Resource subject, Resource body, string code, CancellationToken ct)
    {
        var html = _templates.Render(templateName, new Dictionary<string, string>
        {
            ["appName"] = _appName,
            ["title"] = _localization.GetMessage(subject),
            ["intro"] = _localization.GetMessage(body),
            ["code"] = code,
        });

        return _emailSender.SendAsync(EmailMessage.To1(email, _localization.GetMessage(subject), html), ct);
    }

    private Task<User?> LoadByEmailAsync(string email, CancellationToken ct)
        => _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == Normalize(email) && u.IsActive, ct);

    private Task<User?> LoadTrackedByEmailAsync(string email, CancellationToken ct)
        => _context.Users.FirstOrDefaultAsync(u => u.Email == Normalize(email) && u.IsActive, ct);

    private static string CacheKey(int userId, OtpPurpose purpose) => $"otp:{purpose}:{userId}";

    private static string Normalize(string email) => email.Trim().ToLowerInvariant();

    private static string Hash(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    private static bool FixedTimeEquals(string a, string b)
        => CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));

    private static string GenerateToken() => Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

    private static string GenerateNumericCode(int length)
    {
        length = length < 4 ? 6 : length;
        var digits = new char[length];
        for (var i = 0; i < length; i++)
        {
            digits[i] = (char)('0' + RandomNumberGenerator.GetInt32(10));
        }

        return new string(digits);
    }
}
