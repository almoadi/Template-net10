using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Template_net10.API.Controllers;
using Template_net10.API.Extensions;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Application.Auth.Authentication.Commands.Login;
using Template_net10.Application.Common.Models;

namespace Template_net10.API.Controllers.Auth;

[Route("api/auth")]
public sealed class AuthController : ApiControllerBase
{
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingPolicies.Auth)]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<AuthTokenDto>))]
    public async Task<ActionResult<ApiResponseDto<AuthTokenDto>>> Login([FromBody] LoginCommand command)
        => await Sender.Send(command);
}
