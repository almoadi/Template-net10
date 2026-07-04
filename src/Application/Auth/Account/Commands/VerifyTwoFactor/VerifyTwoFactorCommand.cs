using MediatR;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Account.Commands.VerifyTwoFactor;

public sealed class VerifyTwoFactorCommand : IRequest<ApiResponseDto<AuthTokenDto>>
{
    public string Email { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;
}
