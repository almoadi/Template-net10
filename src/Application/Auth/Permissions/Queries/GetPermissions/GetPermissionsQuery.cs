using MediatR;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Permissions.Queries.GetPermissions;

public sealed class GetPermissionsQuery : IRequest<ApiResponseDto<List<PermissionDto>>>;
