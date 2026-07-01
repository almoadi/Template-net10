using Microsoft.EntityFrameworkCore;
using Template_net10.Domain.Common;
using Template_net10.Infrastructure.Data;

namespace Template_net10.UnitTests.Common;

internal sealed class ScopeTestItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    internal ScopeTestItem WithDeletedAt(DateTime? deletedAt)
    {
        DeletedAt = deletedAt;
        return this;
    }
}

internal sealed class ScopeTestDbContext : GlobalFilteredDbContext
{
    public ScopeTestDbContext(DbContextOptions<ScopeTestDbContext> options) : base(options)
    {
    }

    public DbSet<ScopeTestItem> Items => Set<ScopeTestItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScopeTestItem>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
        });

        ApplyGlobalQueryFilters(modelBuilder);
    }
}

internal static class ScopeTestDbContextFactory
{
    public static ScopeTestDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ScopeTestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ScopeTestDbContext(options);
    }
}
