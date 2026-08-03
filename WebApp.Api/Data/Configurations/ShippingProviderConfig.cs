using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class ShippingProviderConfig : IEntityTypeConfiguration<ShippingProvider>
    {
        public void Configure(EntityTypeBuilder<ShippingProvider> builder)
        {
            builder.HasKey(e => e.ProviderId).HasName("PK__Shipping__B54C687D21FE9F92");

            builder.Property(e => e.ContactPhone).HasMaxLength(50);
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.ProviderName).HasMaxLength(100);
        }
    }
}
