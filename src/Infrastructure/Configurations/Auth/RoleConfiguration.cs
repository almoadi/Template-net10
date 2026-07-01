using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Domain.Common;
using Template_net10.Infrastructure.Configurations;

namespace Template_net10.Infrastructure.Configurations.Auth;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);
        BaseEntityConfiguration.Configure(builder);

        builder.Property(x => x.NameEn).IsRequired().HasMaxLength(LengthConstants.L100);
        builder.Property(x => x.NameAr).IsRequired().HasMaxLength(LengthConstants.L100);
        builder.Property(x => x.IsSystem).IsRequired();

        builder.HasIndex(x => x.NameEn).IsUnique().HasFilter("[DeletedAt] IS NULL");

        builder.Metadata
            .FindNavigation(nameof(Role.RolePermissions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(Role.UserRoles))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
