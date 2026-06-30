using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Template_net10.API.Controllers;
using Template_net10.Application.Auth.Users;
using Template_net10.Application.Auth.Users.Commands.CreateUser;
using Template_net10.Application.Auth.Users.Queries.GetUserById;
using Template_net10.Application.Auth.Users.Queries.SearchUsers;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Auth.Constants;
using Template_net10.Infrastructure.Authorization;

namespace Template_net10.API.Controllers.Auth;

[Route("api/auth/users")]
public sealed class UsersController : ApiControllerBase
{
    [HasPermission(AuthPermissionCodes.UsersWrite)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<int>))]
    public async Task<ActionResult<ApiResponseDto<int>>> Create([FromBody] CreateUserCommand command)
        => await Sender.Send(command);

    [HasPermission(AuthPermissionCodes.UsersRead)]
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<UserDto>))]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> GetById([FromRoute] int id)
        => await Sender.Send(new GetUserByIdQuery { Id = id });

    [HasPermission(AuthPermissionCodes.UsersRead)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedApiResponseDto<UserDto>))]
    public async Task<ActionResult<PagedApiResponseDto<UserDto>>> Search(
        [FromQuery] SearchUsersQuery query)
        => await Sender.Send(query);
}
