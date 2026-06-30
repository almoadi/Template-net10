using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Users.Queries.SearchUsers;

public sealed class SearchUsersQuery : PagedRequest, IRequest<PagedApiResponseDto<UserDto>>
{
    public string? Search { get; set; }
}
