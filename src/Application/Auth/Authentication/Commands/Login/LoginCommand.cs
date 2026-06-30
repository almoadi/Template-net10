using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Authentication.Commands.Login;

public sealed class LoginCommand : IRequest<ApiResponseDto<AuthTokenDto>>
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
