using Template_net10.Application.Common.Extensions;
using Template_net10.Domain.Auth.Entities;

namespace Template_net10.Application.Auth.Roles;

/// <summary>Role-specific query scopes (search columns, system-role filter, soft-delete helpers).</summary>
public static class RoleScopeExtensions
{
    public static IQueryable<Role> SearchRoles(this IQueryable<Role> query, string? term)
        => query.Search(term, r => r.NameEn, r => r.NameAr);

    public static IQueryable<Role> ExcludeSystemRoles(this IQueryable<Role> query)
        => query.Where(r => !r.IsSystem);

    public static IQueryable<Role> WithDeletedRoles(this IQueryable<Role> query) => query.WithTrashed();

    public static IQueryable<Role> OnlyDeletedRoles(this IQueryable<Role> query) => query.OnlyTrashed();
}
