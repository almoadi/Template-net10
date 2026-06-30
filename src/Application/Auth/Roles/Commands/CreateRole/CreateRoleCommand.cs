using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Roles.Commands.CreateRole;

public sealed class CreateRoleCommand : IRequest<ApiResponseDto<int>>
{
    public string NameEn { get; set; } = string.Empty;

    public string NameAr { get; set; } = string.Empty;

    public List<int> PermissionIds { get; set; } = [];
}
