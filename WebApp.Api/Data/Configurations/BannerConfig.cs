using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class BannerConfig : IEntityTypeConfiguration<Banner>
    {
        public void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.HasKey(e => e.BannerId).HasName("PK__Banners__32E86AD1F109AC9C");
            builder.Property(e => e.ImageUrl).HasMaxLength(500);
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.TargetUrl).HasMaxLength(500);
            builder.Property(e => e.Title).HasMaxLength(150);
        }
    }
}
