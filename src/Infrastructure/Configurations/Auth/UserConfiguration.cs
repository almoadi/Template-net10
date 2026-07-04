using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Domain.Common;
using Template_net10.Infrastructure.Configurations;

namespace Template_net10.Infrastructure.Configurations.Auth;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);
        BaseEntityConfiguration.Configure(builder);

        builder.Property(x => x.NameEn).IsRequired().HasMaxLength(LengthConstants.L255);
        builder.Property(x => x.NameAr).IsRequired().HasMaxLength(LengthConstants.L255);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(LengthConstants.L255);
        builder.Property(x => x.Phone).IsRequired().HasMaxLength(LengthConstants.L20);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(LengthConstants.L500);
        builder.Property(x => x.EmailVerifiedAt);
        builder.Property(x => x.TwoFactorEnabled).IsRequired();

        builder.HasIndex(x => x.Email).IsUnique().HasFilter("[DeletedAt] IS NULL");
        builder.HasIndex(x => x.Phone).IsUnique().HasFilter("[DeletedAt] IS NULL AND [Phone] <> ''");

        builder.Metadata
            .FindNavigation(nameof(User.UserRoles))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
