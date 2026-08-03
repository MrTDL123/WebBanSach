using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class PurchaseOrderItemConfig : IEntityTypeConfiguration<PurchaseOrderItem>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
        {
            builder.HasKey(e => e.ItemNo).HasName("PK__Purchase__727D9FE4A98A8E22");

            builder.Property(e => e.ImportUnitPrice).HasColumnType("decimal(18, 2)");

            builder.HasOne(d => d.Product).WithMany(p => p.PurchaseOrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseOrderItems_Products");

            builder.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseOrderItems)
                .HasForeignKey(d => d.PurchaseOrderId)
                .HasConstraintName("FK_PurchaseOrderItems_Orders");
        }
    }
}
