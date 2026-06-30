using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Authentication.Queries.GetMySessions;

public sealed class GetMySessionsQuery : IRequest<ApiResponseDto<IReadOnlyList<UserSessionDto>>>
{
}
