namespace Template_net10.Application.Common.Models;

/// <summary>Standard response envelope returned by every endpoint.</summary>
public class ApiResponseDto<T>
{
    public bool IsSuccess { get; init; }

    public T? Data { get; init; }

    public string? Message { get; init; }

    public IReadOnlyList<string>? Errors { get; init; }

    public static ApiResponseDto<T> Success(T data, string? message = null)
        => new() { IsSuccess = true, Data = data, Message = message };

    public static ApiResponseDto<T> Failed(string? message = null, IReadOnlyList<string>? errors = null)
        => new() { IsSuccess = false, Message = message, Errors = errors };
}
