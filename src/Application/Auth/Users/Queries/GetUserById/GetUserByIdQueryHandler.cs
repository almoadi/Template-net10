using MediatR;
using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ApiResponseDto<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILocalizationService _localization;

    public GetUserByIdQueryHandler(IApplicationDbContext context, ILocalizationService localization)
    {
        _context = context;
        _localization = localization;
    }

    public async Task<ApiResponseDto<UserDto>> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var dto = await _context.Users
            .AsNoTracking()
            .Where(x => x.Id == query.Id)
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
            .FirstOrDefaultAsync(ct);

        return dto is null
            ? ApiResponseDto<UserDto>.Failed(_localization.GetMessage(Resource.UserNotFound))
            : ApiResponseDto<UserDto>.Success(dto);
    }
}
