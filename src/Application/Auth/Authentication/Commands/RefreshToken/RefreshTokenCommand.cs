using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Authentication.Commands.RefreshToken;

public sealed class RefreshTokenCommand : IRequest<ApiResponseDto<AuthTokenDto>>
{
    public string RefreshToken { get; set; } = string.Empty;
}
