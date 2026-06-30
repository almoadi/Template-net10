using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Auth;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Infrastructure.Authorization;

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

    public AuthService(
        IHttpContextAccessor httpContextAccessor,
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService tokenService)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
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

        return new AuthTokenDto { AccessToken = token, ExpiresAtUtc = expiresAt };
    }

    public async Task<bool> Validate(
        string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await LoadByEmailAsync(email, cancellationToken);
        return user is not null && _passwordHasher.Verify(user.PasswordHash, password);
    }

    private Task<Domain.Auth.Entities.User?> LoadByEmailAsync(string email, CancellationToken ct)
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
