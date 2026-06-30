using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Users.Queries.GetUserById;

public sealed class GetUserByIdQuery : IRequest<ApiResponseDto<UserDto>>
{
    public int Id { get; set; }
}
