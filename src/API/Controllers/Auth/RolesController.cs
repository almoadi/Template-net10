using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Template_net10.API.Controllers;
using Template_net10.Application.Auth.Roles;
using Template_net10.Application.Auth.Roles.Commands.CreateRole;
using Template_net10.Application.Auth.Roles.Queries.SearchRoles;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Auth.Constants;
using Template_net10.Infrastructure.Authorization.Permissions;

namespace Template_net10.API.Controllers.Auth;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth/roles")]
public sealed class RolesController : ApiControllerBase
{
    [HasPermission(AuthPermissionCodes.RolesRead)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedApiResponseDto<RoleDto>))]
    public async Task<ActionResult<PagedApiResponseDto<RoleDto>>> Search(
        [FromQuery] SearchRolesQuery query)
        => await Sender.Send(query);

    [HasPermission(AuthPermissionCodes.RolesWrite)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<int>))]
    public async Task<ActionResult<ApiResponseDto<int>>> Create([FromBody] CreateRoleCommand command)
        => await Sender.Send(command);
}
