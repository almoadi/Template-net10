using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Domain.Common;

namespace Template_net10.Infrastructure.Configurations.Auth;

public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.RefreshTokenHash).IsRequired().HasMaxLength(LengthConstants.L255);
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.LastActivityAt).IsRequired();
        builder.Property(x => x.RevokedAt);
        builder.Property(x => x.Device).HasMaxLength(LengthConstants.L255);
        builder.Property(x => x.IpAddress).HasMaxLength(LengthConstants.L50);
        builder.Property(x => x.UserAgent).HasMaxLength(LengthConstants.L500);

        builder.HasIndex(x => x.RefreshTokenHash).IsUnique();
        builder.HasIndex(x => x.UserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
