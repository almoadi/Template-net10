using MediatR;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Auth.Enums;

namespace Template_net10.Application.Auth.SocialLogin.Commands.SocialLogin;

/// <summary>
/// Signs a user in with a social provider access token (Laravel Socialite style). The provider and
/// its access token come from the front-end OAuth flow; the backend validates the token, provisions
/// or links the local user, and returns this application's own access/refresh token pair.
/// </summary>
public sealed class SocialLoginCommand : IRequest<ApiResponseDto<AuthTokenDto>>
{
    public SocialProvider Provider { get; set; }

    public string AccessToken { get; set; } = string.Empty;
}
