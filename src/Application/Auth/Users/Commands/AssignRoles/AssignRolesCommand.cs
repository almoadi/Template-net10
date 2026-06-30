using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Users.Commands.AssignRoles;

public sealed class AssignRolesCommand : IRequest<ApiResponseDto<MessageDto>>
{
    public int UserId { get; set; }

    public List<int> RoleIds { get; set; } = [];
}
