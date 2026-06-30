using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Domain.Common;

namespace Template_net10.Infrastructure.Configurations.Auth;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NameEn).IsRequired().HasMaxLength(LengthConstants.L255);
        builder.Property(x => x.NameAr).IsRequired().HasMaxLength(LengthConstants.L255);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(LengthConstants.L255);
        builder.Property(x => x.Phone).IsRequired().HasMaxLength(LengthConstants.L20);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(LengthConstants.L500);
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.Phone).IsUnique();

        builder.Metadata
            .FindNavigation(nameof(User.UserRoles))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
