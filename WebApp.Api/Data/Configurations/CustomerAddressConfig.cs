using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class CustomerAddressConfig : IEntityTypeConfiguration<CustomerAddress>
    {
        public void Configure(EntityTypeBuilder<CustomerAddress> builder)
        {
            builder.HasKey(e => e.AddressId).HasName("PK__Customer__091C2AFB07D93E46");

            builder.Property(e => e.DetailAddress).HasMaxLength(300);
            builder.Property(e => e.District).HasMaxLength(100);
            builder.Property(e => e.Province).HasMaxLength(100);
            builder.Property(e => e.ReceiverName).HasMaxLength(150);
            builder.Property(e => e.ReceiverPhone).HasMaxLength(50);
            builder.Property(e => e.UserId).HasMaxLength(450);
            builder.Property(e => e.Ward).HasMaxLength(100);

            builder.HasOne(d => d.User).WithMany(p => p.CustomerAddresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_CustomerAddresses_Users");
        }
    }
}
