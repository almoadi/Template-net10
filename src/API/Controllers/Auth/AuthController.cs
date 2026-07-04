using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Template_net10.API.Controllers;
using Template_net10.API.Extensions;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Application.Auth.Authentication.Commands.Login;
using Template_net10.Application.Auth.Authentication.Commands.Logout;
using Template_net10.Application.Auth.Authentication.Commands.LogoutAll;
using Template_net10.Application.Auth.Authentication.Commands.RefreshToken;
using Template_net10.Application.Auth.Authentication.Queries.GetMySessions;
using Template_net10.Application.Common.Models;

namespace Template_net10.API.Controllers.Auth;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : ApiControllerBase
{
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingPolicies.Auth)]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<AuthTokenDto>))]
    public async Task<ActionResult<ApiResponseDto<AuthTokenDto>>> Login([FromBody] LoginCommand command)
        => await Sender.Send(command);

    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingPolicies.Auth)]
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<AuthTokenDto>))]
    public async Task<ActionResult<ApiResponseDto<AuthTokenDto>>> Refresh([FromBody] RefreshTokenCommand command)
        => await Sender.Send(command);

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<MessageDto>))]
    public async Task<ActionResult<ApiResponseDto<MessageDto>>> Logout([FromBody] LogoutCommand command)
        => await Sender.Send(command);

    [Authorize]
    [HttpPost("logout-all")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<MessageDto>))]
    public async Task<ActionResult<ApiResponseDto<MessageDto>>> LogoutAll()
        => await Sender.Send(new LogoutAllCommand());

    [Authorize]
    [HttpGet("sessions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<IReadOnlyList<UserSessionDto>>))]
    public async Task<ActionResult<ApiResponseDto<IReadOnlyList<UserSessionDto>>>> Sessions()
        => await Sender.Send(new GetMySessionsQuery());
}
