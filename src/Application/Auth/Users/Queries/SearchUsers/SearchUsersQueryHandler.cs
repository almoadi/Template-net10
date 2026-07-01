using MediatR;
using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Auth.Users;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Common.Extensions;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Users.Queries.SearchUsers;

public sealed class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, PagedApiResponseDto<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchUsersQueryHandler(IApplicationDbContext context) => _context = context;

    public Task<PagedApiResponseDto<UserDto>> Handle(SearchUsersQuery query, CancellationToken ct)
        => _context.Users
            .AsNoTracking()
            .SearchUsers(query.Search)
            .OrderById()
            .Select(x => new UserDto
            {
                Id = x.Id,
                NameEn = x.NameEn,
                NameAr = x.NameAr,
                Email = x.Email,
                Phone = x.Phone,
                IsActive = x.IsActive,
                Roles = x.UserRoles.Select(ur => ur.Role.NameEn).ToList(),
            })
            .ToPagedResponseAsync(query, ct);
}
