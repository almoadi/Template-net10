using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Domain.Common;
using Template_net10.Infrastructure.Configurations;

namespace Template_net10.Infrastructure.Configurations.Auth;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(x => x.Id);
        BaseEntityConfiguration.Configure(builder);

        builder.Property(x => x.Code).IsRequired().HasMaxLength(LengthConstants.L100);
        builder.Property(x => x.NameEn).IsRequired().HasMaxLength(LengthConstants.L255);
        builder.Property(x => x.NameAr).IsRequired().HasMaxLength(LengthConstants.L255);

        builder.HasIndex(x => x.Code).IsUnique().HasFilter("[DeletedAt] IS NULL");

        builder.Metadata
            .FindNavigation(nameof(Permission.RolePermissions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
