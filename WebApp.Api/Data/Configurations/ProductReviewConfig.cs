using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class ProductReviewConfig : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.HasKey(e => e.ReviewId).HasName("PK__ProductR__74BC79CE19A8A691");

            builder.Property(e => e.Comment).HasMaxLength(1000);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            builder.Property(e => e.CustomerId).HasMaxLength(450);
            builder.Property(e => e.RepliedByEmployeeId).HasMaxLength(450);
            builder.Property(e => e.ReplyFromShop).HasMaxLength(1000);

            builder.HasOne(d => d.Customer).WithMany(p => p.ProductReviewCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductReviews_Customer");

            builder.HasOne(d => d.Product).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductReviews_Products");

            builder.HasOne(d => d.RepliedByEmployee).WithMany(p => p.ProductReviewRepliedByEmployees)
                .HasForeignKey(d => d.RepliedByEmployeeId)
                .HasConstraintName("FK_ProductReviews_Employee");
        }
    }
}
