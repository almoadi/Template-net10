using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template_net10.Domain.Common;

namespace Template_net10.Infrastructure.Configurations;

/// <summary>Shared EF mapping for global-scope columns on <see cref="BaseEntity"/>.</summary>
public static class BaseEntityConfiguration
{
    public static void Configure<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : BaseEntity
    {
        builder.Property(x => x.DeletedAt);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);
    }
}
