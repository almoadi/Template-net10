using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Account.Commands.ResetPassword;

public sealed class ResetPasswordCommand : IRequest<ApiResponseDto<MessageDto>>
{
    public string Email { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;
}
