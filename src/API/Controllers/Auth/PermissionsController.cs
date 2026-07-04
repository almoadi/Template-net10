using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Template_net10.API.Controllers;
using Template_net10.Application.Auth.Permissions;
using Template_net10.Application.Auth.Permissions.Queries.GetPermissions;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Auth.Constants;
using Template_net10.Infrastructure.Authorization.Permissions;

namespace Template_net10.API.Controllers.Auth;

[Route("api/auth/permissions")]
public sealed class PermissionsController : ApiControllerBase
{
    [HasPermission(AuthPermissionCodes.PermissionsRead)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<List<PermissionDto>>))]
    public async Task<ActionResult<ApiResponseDto<List<PermissionDto>>>> GetAll()
        => await Sender.Send(new GetPermissionsQuery());
}
