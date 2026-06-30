using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Roles.Queries.SearchRoles;

public sealed class SearchRolesQuery : PagedRequest, IRequest<PagedApiResponseDto<RoleDto>>
{
    public string? Search { get; set; }
}
