using MediatR;
using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Authentication.Queries.GetMySessions;

public sealed class GetMySessionsQueryHandler
    : IRequestHandler<GetMySessionsQuery, ApiResponseDto<IReadOnlyList<UserSessionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMySessionsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponseDto<IReadOnlyList<UserSessionDto>>> Handle(
        GetMySessionsQuery query, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return ApiResponseDto<IReadOnlyList<UserSessionDto>>.Success([]);
        }

        var now = DateTime.UtcNow;

        var sessions = await _context.UserSessions
            .AsNoTracking()
            .Where(s => s.UserId == userId && s.RevokedAt == null && s.ExpiresAt > now)
            .OrderByDescending(s => s.LastActivityAt)
            .Select(s => new UserSessionDto
            {
                Id = s.Id,
                Device = s.Device,
                IpAddress = s.IpAddress,
                UserAgent = s.UserAgent,
                CreatedAtUtc = s.CreatedAt,
                LastActivityAtUtc = s.LastActivityAt,
                ExpiresAtUtc = s.ExpiresAt,
            })
            .ToListAsync(ct);

        return ApiResponseDto<IReadOnlyList<UserSessionDto>>.Success(sessions);
    }
}
