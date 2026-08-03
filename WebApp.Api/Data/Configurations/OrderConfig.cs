using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF623EC2DA");

            builder.HasIndex(e => e.OrderCode, "UQ__Orders__999B5229FEEFC815").IsUnique();

            builder.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            builder.Property(e => e.Notes).HasMaxLength(500);
            builder.Property(e => e.OrderCode).HasMaxLength(50);
            builder.Property(e => e.OrderDate).HasDefaultValueSql("(getutcdate())");
            builder.Property(e => e.ReceiverName).HasMaxLength(150);
            builder.Property(e => e.ReceiverPhone).HasMaxLength(50);
            builder.Property(e => e.ShippingAddress).HasMaxLength(500);
            builder.Property(e => e.ShippingFee).HasColumnType("decimal(18, 2)");
            builder.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");
            builder.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            builder.Property(e => e.UserId).HasMaxLength(450);

            builder.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Users");
        }
    }
}
