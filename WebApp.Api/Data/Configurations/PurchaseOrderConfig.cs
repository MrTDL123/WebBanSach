using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class PurchaseOrderConfig : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.HasKey(e => e.PurchaseOrderId).HasName("PK__Purchase__036BACA4FF1531CF");

            builder.Property(e => e.CreatedByEmployeeId).HasMaxLength(450);
            builder.Property(e => e.ImportDate).HasDefaultValueSql("(getutcdate())");
            builder.Property(e => e.Notes).HasMaxLength(500);
            builder.Property(e => e.TotalCost).HasColumnType("decimal(18, 2)");

            builder.HasOne(d => d.CreatedByEmployee).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.CreatedByEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseOrders_Employee");

            builder.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseOrders_Suppliers");
        }
    }
}
