using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Domain.Common;

namespace Template_net10.Infrastructure.Configurations.Auth;

public sealed class SocialAccountConfiguration : IEntityTypeConfiguration<SocialAccount>
{
    public void Configure(EntityTypeBuilder<SocialAccount> builder)
    {
        builder.ToTable("SocialAccounts");

        builder.HasKey(x => x.Id);
        BaseEntityConfiguration.Configure(builder);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Provider).IsRequired().HasConversion<string>().HasMaxLength(LengthConstants.L50);
        builder.Property(x => x.ProviderUserId).IsRequired().HasMaxLength(LengthConstants.L255);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(LengthConstants.L255);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(LengthConstants.L255);
        builder.Property(x => x.AvatarUrl).HasMaxLength(LengthConstants.L1000);

        // One row per external identity; re-linking updates the existing row.
        builder.HasIndex(x => new { x.Provider, x.ProviderUserId }).IsUnique().HasFilter("[DeletedAt] IS NULL");
        builder.HasIndex(x => x.UserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
