namespace Template_net10.Application.Common.Models;

/// <summary>Contract for any request that supports limit/offset paging.</summary>
public interface IPagedRequest
{
    int Limit { get; set; }

    int Offset { get; set; }
}

/// <summary>Base class for paged queries. Inherit and add filter properties.</summary>
public abstract class PagedRequest : IPagedRequest
{
    public int Limit { get; set; } = PagedApiResponseFactory.DefaultPageSize;

    public int Offset { get; set; }
}
