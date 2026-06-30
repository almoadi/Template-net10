using MediatR;
using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Permissions.Queries.GetPermissions;

public sealed class GetPermissionsQueryHandler
    : IRequestHandler<GetPermissionsQuery, ApiResponseDto<List<PermissionDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetPermissionsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<ApiResponseDto<List<PermissionDto>>> Handle(
        GetPermissionsQuery query, CancellationToken ct)
    {
        var permissions = await _context.Permissions
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .Select(x => new PermissionDto
            {
                Id = x.Id,
                Code = x.Code,
                NameEn = x.NameEn,
                NameAr = x.NameAr,
            })
            .ToListAsync(ct);

        return ApiResponseDto<List<PermissionDto>>.Success(permissions);
    }
}
