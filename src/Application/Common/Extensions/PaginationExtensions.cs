using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Common.Extensions;

/// <summary>
/// Laravel-style <c>paginate()</c> for <see cref="IQueryable{T}"/>. Handlers filter, order,
/// and project, then call <see cref="ToPagedResponseAsync{T}"/> — never assembling paging
/// metadata by hand.
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Normalizes the page, counts the total, applies Skip/Take, and wraps the rows
    /// in a <see cref="PagedApiResponseDto{T}"/>.
    /// </summary>
    public static async Task<PagedApiResponseDto<T>> ToPagedResponseAsync<T>(
        this IQueryable<T> source,
        IPagedRequest request,
        CancellationToken ct = default) where T : class
    {
        var (limit, offset) = PagedApiResponseFactory.Normalize(request);

        var totalCount = await source.CountAsync(ct);

        var rows = await source
            .Skip(offset)
            .Take(limit)
            .ToListAsync(ct);

        return new PagedApiResponseDto<T>(Metadata.Create(totalCount, limit, offset), rows);
    }
}
