using MediatR;
using Template_net10.Application.Abstractions.Auth.Social;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.SocialLogin.Commands.SocialLogin;

public sealed class SocialLoginCommandHandler : IRequestHandler<SocialLoginCommand, ApiResponseDto<AuthTokenDto>>
{
    private readonly ISocialite _socialite;

    public SocialLoginCommandHandler(ISocialite socialite)
    {
        _socialite = socialite;
    }

    public async Task<ApiResponseDto<AuthTokenDto>> Handle(SocialLoginCommand command, CancellationToken ct)
    {
        var token = await _socialite.LoginWithTokenAsync(command.Provider, command.AccessToken, ct);

        return ApiResponseDto<AuthTokenDto>.Success(token);
    }
}
