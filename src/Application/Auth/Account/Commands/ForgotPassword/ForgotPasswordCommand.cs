using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Account.Commands.ForgotPassword;

public sealed class ForgotPasswordCommand : IRequest<ApiResponseDto<MessageDto>>
{
    public string Email { get; set; } = string.Empty;
}
