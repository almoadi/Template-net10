using MediatR;
using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Common.Exceptions;

namespace Template_net10.Application.Auth.Users.Commands.AssignRoles;

public sealed class AssignRolesCommandHandler
    : IRequestHandler<AssignRolesCommand, ApiResponseDto<MessageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILocalizationService _localization;

    public AssignRolesCommandHandler(IApplicationDbContext context, ILocalizationService localization)
    {
        _context = context;
        _localization = localization;
    }

    public async Task<ApiResponseDto<MessageDto>> Handle(AssignRolesCommand command, CancellationToken ct)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == command.UserId, ct)
            ?? throw new ItemNotFoundException(_localization.GetMessage(Resource.UserNotFound));

        var roles = await _context.Roles
            .Where(r => command.RoleIds.Contains(r.Id))
            .ToListAsync(ct);

        user.ClearRoles();
        foreach (var role in roles)
        {
            user.AssignRole(role);
        }

        await _context.SaveChangesAsync(ct);

        return ApiResponseDto<MessageDto>.Success(
            MessageDto.Of(_localization.GetMessage(Resource.RolesAssigned)));
    }
}
