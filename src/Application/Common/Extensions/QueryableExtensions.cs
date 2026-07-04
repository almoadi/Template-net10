using System.Linq.Expressions;

namespace Template_net10.Application.Common.Extensions;

/// <summary>Composable <see cref="IQueryable{T}"/> helpers for building queries with optional filters.</summary>
public static class QueryableExtensions
{
    /// <summary>Applies <paramref name="predicate"/> only when <paramref name="condition"/> is true.</summary>
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
        => condition ? source.Where(predicate) : source;
}
