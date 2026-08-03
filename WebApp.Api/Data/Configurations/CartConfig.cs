using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class CartConfig : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(e => e.CartId).HasName("PK__Carts__51BCD7B7C44EDEA0");

            builder.HasIndex(e => e.UserId, "UQ__Carts__1788CC4D3E48C118").IsUnique();

            builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");

            builder.HasOne(d => d.User).WithOne(p => p.Cart)
                .HasForeignKey<Cart>(d => d.UserId)
                .HasConstraintName("FK_Carts_Users");
        }
    }
}
