using Template_net10.Application.Common.Extensions;
using Template_net10.Domain.Auth.Entities;

namespace Template_net10.Application.Auth.Users;

/// <summary>User-specific query scopes (search columns + soft-delete helpers).</summary>
public static class UserScopeExtensions
{
    public static IQueryable<User> SearchUsers(this IQueryable<User> query, string? term)
        => query.Search(term, u => u.NameEn, u => u.NameAr, u => u.Email, u => u.Phone);

    public static IQueryable<User> WithDeletedUsers(this IQueryable<User> query) => query.WithTrashed();

    public static IQueryable<User> OnlyDeletedUsers(this IQueryable<User> query) => query.OnlyTrashed();
}
