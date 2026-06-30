namespace Template_net10.Application.Common.Models;

/// <summary>Paging metadata attached to a <see cref="PagedApiResponseDto{T}"/>.</summary>
public sealed class Metadata
{
    public required ResultSet ResultSet { get; init; }

    public static Metadata Create(int count, int limit, int offset)
        => new() { ResultSet = new ResultSet { Count = count, Limit = limit, Offset = offset } };
}

/// <summary>Counts describing the full result set behind a single page.</summary>
public sealed class ResultSet
{
    /// <summary>Total rows matching the filter (not just the current page).</summary>
    public int Count { get; init; }

    /// <summary>Page size actually applied (after normalization).</summary>
    public int Limit { get; init; }

    /// <summary>Rows skipped before the current page.</summary>
    public int Offset { get; init; }
}
