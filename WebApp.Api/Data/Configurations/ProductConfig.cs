using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CD83DABE81");

            builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            builder.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.MainImageUrl).HasMaxLength(500);
            builder.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            builder.Property(e => e.SupplierName).HasMaxLength(150);
            builder.Property(e => e.Title).HasMaxLength(255);
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");

            builder.HasOne(d => d.Author).WithMany(p => p.Products)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Authors");

            builder.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");

            builder.HasOne(d => d.Publisher).WithMany(p => p.Products)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Publishers");
        }
    }
}
