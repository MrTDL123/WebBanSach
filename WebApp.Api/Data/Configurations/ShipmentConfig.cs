using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class ShipmentConfig : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.HasKey(e => e.ShipmentId).HasName("PK__Shipment__5CAD37EDE1870D97");

            builder.HasIndex(e => e.OrderId, "UQ__Shipment__C3905BCEB9FE9560").IsUnique();

            builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            builder.Property(e => e.ProcessedByEmployeeId).HasMaxLength(450);
            builder.Property(e => e.ShippingStatus)
                .HasMaxLength(50)
                .HasDefaultValue("WaitingForPickup");
            builder.Property(e => e.TrackingNumber).HasMaxLength(100);

            builder.HasOne(d => d.Order).WithOne(p => p.Shipment)
                .HasForeignKey<Shipment>(d => d.OrderId)
                .HasConstraintName("FK_Shipments_Orders");

            builder.HasOne(d => d.ProcessedByEmployee).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.ProcessedByEmployeeId)
                .HasConstraintName("FK_Shipments_Employee");

            builder.HasOne(d => d.Provider).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Shipments_Providers");
        }
    }
}
