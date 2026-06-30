using Microsoft.EntityFrameworkCore;
using Template_net10.Domain.Auth.Entities;

namespace Template_net10.Application.Abstractions.Data;

/// <summary>
/// Write/read surface over the persistence store exposed to Application handlers.
/// Declared here (not in Infrastructure) so handlers depend on an abstraction, and
/// implemented by the EF Core <c>ApplicationDbContext</c> in Infrastructure.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }

    DbSet<Role> Roles { get; }

    DbSet<Permission> Permissions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
