namespace Template_net10.Application.Common.Models;

/// <summary>
/// Centralizes the paging policy (default/max page size and bound clamping) so every
/// paged endpoint behaves identically. Change paging behaviour in one place.
/// </summary>
public static class PagedApiResponseFactory
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    /// <summary>Clamps a request's Limit/Offset into safe bounds.</summary>
    public static (int Limit, int Offset) Normalize(IPagedRequest request)
    {
        var limit = request.Limit <= 0 ? DefaultPageSize : Math.Min(request.Limit, MaxPageSize);
        var offset = request.Offset < 0 ? 0 : request.Offset;
        return (limit, offset);
    }
}
