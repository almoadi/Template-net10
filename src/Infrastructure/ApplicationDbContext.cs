using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Infrastructure.Data;

namespace Template_net10.Infrastructure;

/// <summary>EF Core implementation of <see cref="IApplicationDbContext"/>.</summary>
public sealed class ApplicationDbContext : GlobalFilteredDbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserSession> UserSessions => Set<UserSession>();

    public DbSet<SocialAccount> SocialAccounts => Set<SocialAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        ApplyGlobalQueryFilters(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}
