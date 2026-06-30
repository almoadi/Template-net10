namespace Template_net10.Application.Common.Models;

/// <summary>
/// Paged response envelope. Derives from <see cref="ApiResponseDto{T}"/> of a list and
/// adds a <see cref="Metadata"/> block carrying paging info.
/// </summary>
public sealed class PagedApiResponseDto<T> : ApiResponseDto<List<T>>
{
    public PagedApiResponseDto(Metadata metaData, List<T> data)
    {
        MetaData = metaData;
        Data = data;
        IsSuccess = true;
    }

    public Metadata MetaData { get; init; }
}
