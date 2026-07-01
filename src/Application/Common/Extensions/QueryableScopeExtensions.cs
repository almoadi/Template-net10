using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Template_net10.Domain.Common;

namespace Template_net10.Application.Common.Extensions;

/// <summary>
/// Generic local query scopes for any entity (Eloquent <c>scopeX</c> equivalent).
/// Global soft-delete scope is applied in EF via <see cref="GlobalFilteredDbContext"/> for every <see cref="BaseEntity"/>.
/// </summary>
public static class QueryableScopeExtensions
{
    // ── Global scope bypass (Eloquent withoutGlobalScope / withTrashed) ───────────────

    /// <summary>Bypasses all EF global query filters on this query.</summary>
    public static IQueryable<T> WithoutGlobalScopes<T>(this IQueryable<T> query) where T : class
        => query.IgnoreQueryFilters();

    /// <summary>Alias for <see cref="WithoutGlobalScopes{T}"/> — includes soft-deleted rows when that global scope is active.</summary>
    public static IQueryable<T> WithTrashed<T>(this IQueryable<T> query) where T : class
        => query.IgnoreQueryFilters();

    /// <summary>Returns only soft-deleted rows (requires bypassing the soft-delete global filter first).</summary>
    public static IQueryable<T> OnlyTrashed<T>(this IQueryable<T> query) where T : BaseEntity
        => query.IgnoreQueryFilters().Where(e => e.DeletedAt != null);

    // ── Search ────────────────────────────────────────────────────────────────────────

    /// <summary>Filters rows where any given string column contains <paramref name="term"/>.</summary>
    public static IQueryable<T> Search<T>(
        this IQueryable<T> query,
        string? term,
        params Expression<Func<T, string>>[] columns)
    {
        if (string.IsNullOrWhiteSpace(term) || columns.Length == 0)
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(T), "e");
        Expression? combined = null;

        foreach (var column in columns)
        {
            var member = ReplaceParameter(column.Body, column.Parameters[0], parameter);
            var contains = Expression.Call(
                member,
                typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!,
                Expression.Constant(term));

            combined = combined is null ? contains : Expression.OrElse(combined, contains);
        }

        return query.Where(Expression.Lambda<Func<T, bool>>(combined!, parameter));
    }

    // ── Ordering ──────────────────────────────────────────────────────────────────────

    public static IQueryable<T> OrderById<T>(this IQueryable<T> query) where T : BaseEntity
        => query.OrderBy(e => e.Id);

    public static IQueryable<T> OrderByIdDescending<T>(this IQueryable<T> query) where T : BaseEntity
        => query.OrderByDescending(e => e.Id);

    public static IQueryable<T> OrderByCreated<T>(this IQueryable<T> query) where T : BaseEntity
        => query.OrderBy(e => e.CreatedAt);

    public static IQueryable<T> OrderByCreatedDescending<T>(this IQueryable<T> query) where T : BaseEntity
        => query.OrderByDescending(e => e.CreatedAt);

    public static IQueryable<T> OrderByUpdatedDescending<T>(this IQueryable<T> query) where T : BaseEntity
        => query.OrderByDescending(e => e.UpdatedAt);

    // ── Boolean / active filters ────────────────────────────────────────────────────────

    public static IQueryable<T> WhereActive<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>> isActive)
        => query.Where(isActive);

    public static IQueryable<T> ActiveOnly<T>(this IQueryable<T> query) where T : class, IActivatable
        => query.Where(e => e.IsActive);

    public static IQueryable<T> InactiveOnly<T>(this IQueryable<T> query) where T : class, IActivatable
        => query.Where(e => !e.IsActive);

    // ── Equality / set filters ────────────────────────────────────────────────────────

    public static IQueryable<T> WhereEquals<T, TProperty>(
        this IQueryable<T> query,
        Expression<Func<T, TProperty>> property,
        TProperty value)
    {
        var parameter = property.Parameters[0];
        var body = Expression.Equal(property.Body, Expression.Constant(value, typeof(TProperty)));
        return query.Where(Expression.Lambda<Func<T, bool>>(body, parameter));
    }

    public static IQueryable<T> WhereIn<T, TProperty>(
        this IQueryable<T> query,
        Expression<Func<T, TProperty>> property,
        IEnumerable<TProperty> values)
    {
        var valueList = values.ToList();
        if (valueList.Count == 0)
        {
            return query.Where(_ => false);
        }

        var parameter = property.Parameters[0];
        var contains = Expression.Call(
            typeof(Enumerable),
            nameof(Enumerable.Contains),
            [typeof(TProperty)],
            Expression.Constant(valueList),
            property.Body);

        return query.Where(Expression.Lambda<Func<T, bool>>(contains, parameter));
    }

    // ── Id filters ──────────────────────────────────────────────────────────────────────

    public static IQueryable<T> WhereId<T>(this IQueryable<T> query, int id) where T : BaseEntity
        => query.Where(e => e.Id == id);

    public static IQueryable<T> WhereIds<T>(this IQueryable<T> query, IEnumerable<int> ids) where T : BaseEntity
        => query.WhereIn(e => e.Id, ids);

    // ── Date filters ────────────────────────────────────────────────────────────────────

    public static IQueryable<T> CreatedAfter<T>(this IQueryable<T> query, DateTime value) where T : BaseEntity
        => query.Where(e => e.CreatedAt >= value);

    public static IQueryable<T> CreatedBefore<T>(this IQueryable<T> query, DateTime value) where T : BaseEntity
        => query.Where(e => e.CreatedAt <= value);

    public static IQueryable<T> UpdatedAfter<T>(this IQueryable<T> query, DateTime value) where T : BaseEntity
        => query.Where(e => e.UpdatedAt != null && e.UpdatedAt >= value);

    // ── Helpers ─────────────────────────────────────────────────────────────────────────

    private static Expression ReplaceParameter(
        Expression expression,
        ParameterExpression source,
        ParameterExpression target)
        => new ParameterReplacer(source, target).Visit(expression);

    private sealed class ParameterReplacer(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
            => node == source ? target : base.VisitParameter(node);
    }
}
