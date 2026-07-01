using MediatR;
using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Auth.Roles;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Common.Extensions;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Roles.Queries.SearchRoles;

public sealed class SearchRolesQueryHandler
    : IRequestHandler<SearchRolesQuery, PagedApiResponseDto<RoleDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchRolesQueryHandler(IApplicationDbContext context) => _context = context;

    public Task<PagedApiResponseDto<RoleDto>> Handle(SearchRolesQuery query, CancellationToken ct)
        => _context.Roles
            .AsNoTracking()
            .SearchRoles(query.Search)
            .ExcludeSystemRoles()
            .OrderById()
            .Select(x => new RoleDto
            {
                Id = x.Id,
                NameEn = x.NameEn,
                NameAr = x.NameAr,
                IsSystem = x.IsSystem,
                Permissions = x.RolePermissions.Select(rp => rp.Permission.Code).ToList(),
            })
            .ToPagedResponseAsync(query, ct);
}
