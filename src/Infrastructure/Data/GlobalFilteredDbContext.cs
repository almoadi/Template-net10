using Microsoft.EntityFrameworkCore;
using Template_net10.Domain.Common;

namespace Template_net10.Infrastructure.Data;

/// <summary>
/// DbContext base that applies the soft-delete global query filter to every <see cref="BaseEntity"/>.
/// </summary>
public abstract class GlobalFilteredDbContext : DbContext
{
    protected GlobalFilteredDbContext(DbContextOptions options) : base(options)
    {
    }

    protected void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.ClrType is null || !typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            ApplyGlobalQueryFilterForEntity(modelBuilder, entityType.ClrType);
        }
    }

    private void ApplyGlobalQueryFilterForEntity(ModelBuilder modelBuilder, Type clrType)
    {
        var method = typeof(GlobalFilteredDbContext)
            .GetMethod(nameof(ApplyGlobalQueryFilterForEntityGeneric), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .MakeGenericMethod(clrType);

        method.Invoke(this, [modelBuilder]);
    }

    private void ApplyGlobalQueryFilterForEntityGeneric<TEntity>(ModelBuilder modelBuilder)
        where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.DeletedAt == null);
    }
}
