using Microsoft.EntityFrameworkCore;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Domain.Auth.Entities;

namespace Template_net10.Infrastructure;

/// <summary>EF Core implementation of <see cref="IApplicationDbContext"/>.</summary>
public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
