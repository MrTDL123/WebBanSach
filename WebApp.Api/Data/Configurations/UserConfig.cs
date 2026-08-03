using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__Users__3214EC07B299B008");

            builder.Property(e => e.Address).HasMaxLength(300);
            builder.Property(e => e.AvatarUrl).HasMaxLength(500);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            builder.Property(e => e.Email).HasMaxLength(256);
            builder.Property(e => e.FullName).HasMaxLength(150);
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.LockoutEnabled).HasDefaultValue(true);
            builder.Property(e => e.NormalizedEmail).HasMaxLength(256);
            builder.Property(e => e.NormalizedUserName).HasMaxLength(256);
            builder.Property(e => e.PhoneNumber).HasMaxLength(50);
            builder.Property(e => e.UserName).HasMaxLength(256);
        }
    }
}
