using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Template_net10.API.Controllers;
using Template_net10.API.Extensions;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Application.Auth.SocialLogin.Commands.SocialLogin;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Auth.Enums;

namespace Template_net10.API.Controllers.Auth;

/// <summary>
/// Social login (Laravel Socialite style). Token-based flow: the client obtains a provider access
/// token on the front end, then posts it here to receive this application's own JWT.
/// </summary>
[Route("api/auth/social")]
public sealed class SocialAuthController : ApiControllerBase
{
    /// <summary>Exchanges a provider access token for the application's access/refresh token pair.</summary>
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingPolicies.Auth)]
    [HttpPost("{provider}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<AuthTokenDto>))]
    public async Task<ActionResult<ApiResponseDto<AuthTokenDto>>> Login(
        [FromRoute] SocialProvider provider, [FromBody] SocialLoginRequest request)
        => await Sender.Send(new SocialLoginCommand { Provider = provider, AccessToken = request.AccessToken });
}

/// <summary>Body for a social login request — the provider access token from the front-end OAuth flow.</summary>
public sealed class SocialLoginRequest
{
    public string AccessToken { get; set; } = string.Empty;
}
