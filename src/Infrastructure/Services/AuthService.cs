using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Auth;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Infrastructure.Authorization.Permissions;
using Template_net10.Infrastructure.Options;

namespace Template_net10.Infrastructure.Services;

/// <summary>
/// Backing implementation of the Laravel-style <see cref="IAuth"/> facade. Reads identity and
/// abilities from the current request's JWT, and performs credential checks / token issuance against
/// the database. Also satisfies <see cref="ICurrentUserService"/> so the rest of the app has a single
/// source of truth for "who is the caller".
/// </summary>
public sealed class AuthService : IAuth, ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _tokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        IHttpContextAccessor httpContextAccessor,
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService tokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public int? Id
    {
        get
        {
            var value = Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : null;
        }
    }

    public bool Check => Principal?.Identity?.IsAuthenticated ?? false;

    public bool Guest => !Check;

    public IReadOnlyList<string> Roles =>
        Principal?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? [];

    public IReadOnlyList<string> Permissions =>
        Principal?.FindAll(PermissionConstants.ClaimType).Select(c => c.Value).ToList() ?? [];

    public bool HasRole(string role) => Roles.Contains(role);

    public bool Can(string permission) => Permissions.Contains(permission);

    // ICurrentUserService — same identity, exposed under the existing contract.
    public int? UserId => Id;

    public bool IsAuthenticated => Check;

    public async Task<CurrentUserDto?> User(CancellationToken cancellationToken = default)
    {
        if (Id is not { } userId)
        {
            return null;
        }

        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new CurrentUserDto
            {
                Id = u.Id,
                NameEn = u.NameEn,
                NameAr = u.NameAr,
                Email = u.Email,
                Roles = u.UserRoles.Select(ur => ur.Role.NameEn).ToList(),
                Permissions = u.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Code)
                    .Distinct()
                    .ToList(),
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AuthTokenDto?> Attempt(
        string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await LoadByEmailAsync(email, cancellationToken);
        if (user is null || !_passwordHasher.Verify(user.PasswordHash, password))
        {
            return null;
        }

        var (refreshToken, refreshHash) = GenerateRefreshToken();
        var refreshExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays);
        var (device, ipAddress, userAgent) = CurrentRequestInfo();

        _context.UserSessions.Add(
            UserSession.Create(user.Id, refreshHash, refreshExpiresAt, device, ipAddress, userAgent));
        await _context.SaveChangesAsync(cancellationToken);

        return IssueToken(user, refreshToken, refreshExpiresAt);
    }

    public async Task<bool> Validate(
        string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await LoadByEmailAsync(email, cancellationToken);
        return user is not null && _passwordHasher.Verify(user.PasswordHash, password);
    }

    public async Task<AuthTokenDto?> Refresh(
        string refreshToken, CancellationToken cancellationToken = default)
    {
        var hash = HashToken(refreshToken);

        var session = await _context.UserSessions
            .FirstOrDefaultAsync(s => s.RefreshTokenHash == hash, cancellationToken);
        if (session is null || !session.IsActive)
        {
            return null;
        }

        var user = await LoadByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            session.Revoke();
            await _context.SaveChangesAsync(cancellationToken);
            return null;
        }

        var (newRefreshToken, newRefreshHash) = GenerateRefreshToken();
        var refreshExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays);
        session.Rotate(newRefreshHash, refreshExpiresAt);
        await _context.SaveChangesAsync(cancellationToken);

        return IssueToken(user, newRefreshToken, refreshExpiresAt);
    }

    public async Task<bool> Logout(
        string refreshToken, CancellationToken cancellationToken = default)
    {
        var hash = HashToken(refreshToken);

        var session = await _context.UserSessions
            .FirstOrDefaultAsync(s => s.RefreshTokenHash == hash, cancellationToken);
        if (session is null)
        {
            return false;
        }

        session.Revoke();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> LogoutAll(CancellationToken cancellationToken = default)
    {
        if (Id is not { } userId)
        {
            return 0;
        }

        var sessions = await _context.UserSessions
            .Where(s => s.UserId == userId && s.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke();
        }

        await _context.SaveChangesAsync(cancellationToken);
        return sessions.Count;
    }

    private AuthTokenDto IssueToken(User user, string refreshToken, DateTime refreshExpiresAt)
    {
        var roles = user.UserRoles
            .Select(ur => ur.Role.NameEn)
            .Distinct()
            .ToArray();

        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToArray();

        var (token, expiresAt) = _tokenService.GenerateToken(user.Id, roles, permissions);

        return new AuthTokenDto
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAtUtc = refreshExpiresAt,
        };
    }

    private (string Token, string Hash) GenerateRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        return (token, HashToken(token));
    }

    private static string HashToken(string token)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    private (string? Device, string? IpAddress, string? UserAgent) CurrentRequestInfo()
    {
        var http = _httpContextAccessor.HttpContext;
        if (http is null)
        {
            return (null, null, null);
        }

        var device = http.Request.Headers["X-Device-Name"].ToString();
        var ipAddress = http.Connection.RemoteIpAddress?.ToString();
        var userAgent = http.Request.Headers.UserAgent.ToString();

        return (
            string.IsNullOrWhiteSpace(device) ? null : device,
            string.IsNullOrWhiteSpace(ipAddress) ? null : ipAddress,
            string.IsNullOrWhiteSpace(userAgent) ? null : userAgent);
    }

    private Task<User?> LoadByIdAsync(int userId, CancellationToken ct)
        => _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive, ct);

    private Task<User?> LoadByEmailAsync(string email, CancellationToken ct)
    {
        var normalized = email.Trim().ToLowerInvariant();

        return _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email == normalized && u.IsActive, ct);
    }
}
