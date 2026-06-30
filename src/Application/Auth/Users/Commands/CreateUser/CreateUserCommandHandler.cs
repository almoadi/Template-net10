using MediatR;
using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Domain.Common.Exceptions;

namespace Template_net10.Application.Auth.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponseDto<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILocalizationService _localization;

    public CreateUserCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ILocalizationService localization)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _localization = localization;
    }

    public async Task<ApiResponseDto<int>> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var email = command.Email.Trim().ToLowerInvariant();

        if (await _context.Users.AnyAsync(x => x.Email == email, ct))
        {
            throw new BadRequestException(_localization.GetMessage(Resource.EmailAlreadyExists));
        }

        if (await _context.Users.AnyAsync(x => x.Phone == command.Phone, ct))
        {
            throw new BadRequestException(_localization.GetMessage(Resource.PhoneAlreadyExists));
        }

        var entity = User.Create(
            command.NameEn,
            command.NameAr,
            command.Email,
            command.Phone,
            _passwordHasher.Hash(command.Password));

        if (command.RoleIds.Count != 0)
        {
            var roles = await _context.Roles
                .Where(r => command.RoleIds.Contains(r.Id))
                .ToListAsync(ct);

            foreach (var role in roles)
            {
                entity.AssignRole(role);
            }
        }

        _context.Users.Add(entity);
        await _context.SaveChangesAsync(ct);

        return ApiResponseDto<int>.Success(entity.Id, _localization.GetMessage(Resource.UserCreated));
    }
}
