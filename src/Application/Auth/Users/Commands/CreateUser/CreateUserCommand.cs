using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Users.Commands.CreateUser;

public sealed class CreateUserCommand : IRequest<ApiResponseDto<int>>
{
    public string NameEn { get; set; } = string.Empty;

    public string NameAr { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public List<int> RoleIds { get; set; } = [];
}
