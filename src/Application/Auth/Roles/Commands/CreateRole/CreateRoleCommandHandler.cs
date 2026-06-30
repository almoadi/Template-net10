using MediatR;
using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Auth.Entities;

namespace Template_net10.Application.Auth.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ApiResponseDto<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILocalizationService _localization;

    public CreateRoleCommandHandler(IApplicationDbContext context, ILocalizationService localization)
    {
        _context = context;
        _localization = localization;
    }

    public async Task<ApiResponseDto<int>> Handle(CreateRoleCommand command, CancellationToken ct)
    {
        var role = Role.Create(command.NameEn, command.NameAr);

        if (command.PermissionIds.Count != 0)
        {
            var permissions = await _context.Permissions
                .Where(p => command.PermissionIds.Contains(p.Id))
                .ToListAsync(ct);

            role.SetPermissions(permissions);
        }

        _context.Roles.Add(role);
        await _context.SaveChangesAsync(ct);

        return ApiResponseDto<int>.Success(role.Id, _localization.GetMessage(Resource.RoleCreated));
    }
}
