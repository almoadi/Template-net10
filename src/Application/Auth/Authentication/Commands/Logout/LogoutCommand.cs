using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Authentication.Commands.Logout;

public sealed class LogoutCommand : IRequest<ApiResponseDto<MessageDto>>
{
    public string RefreshToken { get; set; } = string.Empty;
}
