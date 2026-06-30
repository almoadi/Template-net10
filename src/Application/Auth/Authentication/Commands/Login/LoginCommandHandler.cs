using MediatR;
using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Common.Exceptions;

namespace Template_net10.Application.Auth.Authentication.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponseDto<AuthTokenDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _tokenService;
    private readonly ILocalizationService _localization;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService tokenService,
        ILocalizationService localization)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _localization = localization;
    }

    public async Task<ApiResponseDto<AuthTokenDto>> Handle(LoginCommand command, CancellationToken ct)
    {
        var email = command.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive, ct);

        if (user is null || !_passwordHasher.Verify(user.PasswordHash, command.Password))
        {
            throw new BadRequestException(_localization.GetMessage(Resource.InvalidCredentials));
        }

        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToArray();

        var (token, expiresAt) = _tokenService.GenerateToken(user.Id, permissions);

        return ApiResponseDto<AuthTokenDto>.Success(new AuthTokenDto
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAt,
        });
    }
}
