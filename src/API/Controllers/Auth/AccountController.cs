using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Template_net10.API.Controllers;
using Template_net10.API.Extensions;
using Template_net10.Application.Auth.Account.Commands.ForgotPassword;
using Template_net10.Application.Auth.Account.Commands.RequestEmailVerification;
using Template_net10.Application.Auth.Account.Commands.ResetPassword;
using Template_net10.Application.Auth.Account.Commands.VerifyEmail;
using Template_net10.Application.Auth.Account.Commands.VerifyTwoFactor;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Application.Common.Models;

namespace Template_net10.API.Controllers.Auth;

/// <summary>
/// Optional account-security endpoints: email verification, password reset, and two-factor login.
/// These behave as no-ops in effect until the matching flags are enabled in <c>config/auth.json</c>.
/// </summary>
[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
public sealed class AccountController : ApiControllerBase
{
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingPolicies.Auth)]
    [HttpPost("email/verify/request")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<MessageDto>))]
    public async Task<ActionResult<ApiResponseDto<MessageDto>>> RequestEmailVerification(
        [FromBody] RequestEmailVerificationCommand command)
        => await Sender.Send(command);

    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingPolicies.Auth)]
    [HttpPost("email/verify")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<MessageDto>))]
    public async Task<ActionResult<ApiResponseDto<MessageDto>>> VerifyEmail([FromBody] VerifyEmailCommand command)
        => await Sender.Send(command);

    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingPolicies.Auth)]
    [HttpPost("password/forgot")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<MessageDto>))]
    public async Task<ActionResult<ApiResponseDto<MessageDto>>> ForgotPassword([FromBody] ForgotPasswordCommand command)
        => await Sender.Send(command);

    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingPolicies.Auth)]
    [HttpPost("password/reset")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<MessageDto>))]
    public async Task<ActionResult<ApiResponseDto<MessageDto>>> ResetPassword([FromBody] ResetPasswordCommand command)
        => await Sender.Send(command);

    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingPolicies.Auth)]
    [HttpPost("2fa/verify")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<AuthTokenDto>))]
    public async Task<ActionResult<ApiResponseDto<AuthTokenDto>>> VerifyTwoFactor([FromBody] VerifyTwoFactorCommand command)
        => await Sender.Send(command);
}
