namespace Template_net10.Application.Common.Extensions;

/// <summary>Small, allocation-friendly helpers for working with sequences.</summary>
public static class EnumerableExtensions
{
    /// <summary>Applies <paramref name="predicate"/> only when <paramref name="condition"/> is true.</summary>
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source, bool condition, Func<T, bool> predicate)
        => condition ? source.Where(predicate) : source;

    /// <summary>Runs <paramref name="action"/> for each item.</summary>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }

    /// <summary>Returns the sequence as an <see cref="IReadOnlyList{T}"/> without re-materializing when possible.</summary>
    public static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> source)
        => source as IReadOnlyList<T> ?? source.ToList();

    /// <summary>True when the sequence is <c>null</c> or contains no elements.</summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        => source is null || !source.Any();
}
