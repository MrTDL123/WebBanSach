using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.JwtId)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())");

            builder.Property(e => e.ExpiresAt)
                .IsRequired();

            builder.Property(e => e.ReplacedByToken)
                .HasMaxLength(500);

            builder.Property(e => e.DeviceInfo)
                .HasMaxLength(250);

            // Bỏ qua các thuộc tính tính toán (Computed Properties) trong EF Core mapping
            builder.Ignore(e => e.IsExpired);
            builder.Ignore(e => e.IsRevoked);
            builder.Ignore(e => e.IsActive);

            // Index cột Token để tối ưu tra cứu khi refresh / revoke token
            builder.HasIndex(e => e.Token)
                .IsUnique();

            // Cấu hình quan hệ với User
            builder.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
