using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Infrastructure.Options;

namespace Template_net10.Infrastructure.Services.Auth;

/// <summary>Creates a persisted refresh session and issues a JWT for an already-authenticated user.</summary>
public sealed class TokenIssuer : ITokenIssuer
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtOptions _jwtOptions;

    public TokenIssuer(
        IApplicationDbContext context,
        IJwtTokenService tokenService,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtOptions> jwtOptions)
    {
        _context = context;
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthTokenDto?> IssueAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var refreshHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        var refreshExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays);
        var (device, ipAddress, userAgent) = CurrentRequestInfo();

        _context.UserSessions.Add(
            UserSession.Create(user.Id, refreshHash, refreshExpiresAt, device, ipAddress, userAgent));
        await _context.SaveChangesAsync(cancellationToken);

        var roles = user.UserRoles.Select(ur => ur.Role.NameEn).Distinct().ToArray();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToArray();

        var (accessToken, expiresAt) = _tokenService.GenerateToken(user.Id, roles, permissions);

        return new AuthTokenDto
        {
            AccessToken = accessToken,
            ExpiresAtUtc = expiresAt,
            RefreshToken = token,
            RefreshTokenExpiresAtUtc = refreshExpiresAt,
        };
    }

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
}
