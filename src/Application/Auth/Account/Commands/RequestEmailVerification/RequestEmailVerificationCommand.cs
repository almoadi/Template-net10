using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Account.Commands.RequestEmailVerification;

public sealed class RequestEmailVerificationCommand : IRequest<ApiResponseDto<MessageDto>>
{
    public string Email { get; set; } = string.Empty;
}
