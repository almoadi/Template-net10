using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Authentication.Commands.LogoutAll;

public sealed class LogoutAllCommand : IRequest<ApiResponseDto<MessageDto>>
{
}
