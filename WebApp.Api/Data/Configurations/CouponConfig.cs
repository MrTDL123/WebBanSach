using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class CouponConfig : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(e => e.CouponId).HasName("PK__Coupons__384AF1BA4F50F068");

            builder.HasIndex(e => e.Code, "UQ__Coupons__A25C5AA7E1398FE0").IsUnique();

            builder.Property(e => e.Code).HasMaxLength(50);
            builder.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.MinOrderAmount).HasColumnType("decimal(18, 2)");
            builder.Property(e => e.UsageLimit).HasDefaultValue(100);
        }
    }
}
