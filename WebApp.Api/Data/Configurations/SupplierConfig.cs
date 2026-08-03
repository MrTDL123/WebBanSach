using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class SupplierConfig : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B416B2B981");

            builder.Property(e => e.Address).HasMaxLength(300);
            builder.Property(e => e.ContactPhone).HasMaxLength(50);
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.SupplierName).HasMaxLength(150);
        }
    }
}
