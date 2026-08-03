using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class BookRequestConfig : IEntityTypeConfiguration<BookRequest>
    {
        public void Configure(EntityTypeBuilder<BookRequest> builder)
        {
            builder.HasKey(e => e.RequestId).HasName("PK__BookRequ__33A8517AD0C6F767");

            builder.HasIndex(e => e.RequestCode, "UQ__BookRequ__CBAB82F6316BA242").IsUnique();

            builder.Property(e => e.CustomerId).HasMaxLength(450);
            builder.Property(e => e.EmployeeNote).HasMaxLength(500);
            builder.Property(e => e.IntendedUse).HasMaxLength(1000);
            builder.Property(e => e.QuantityRequested).HasDefaultValue(1);
            builder.Property(e => e.Reason).HasMaxLength(2000);
            builder.Property(e => e.ReceiverName).HasMaxLength(150);
            builder.Property(e => e.ReceiverPhone).HasMaxLength(20);
            builder.Property(e => e.RejectedReason).HasMaxLength(500);
            builder.Property(e => e.RequestCode).HasMaxLength(20);
            builder.Property(e => e.RequestedAt).HasDefaultValueSql("(getutcdate())");
            builder.Property(e => e.ReviewedByEmployeeId).HasMaxLength(450);
            builder.Property(e => e.ShipmentTrackingNumber).HasMaxLength(100);
            builder.Property(e => e.ShippingAddress).HasMaxLength(500);

            builder.HasOne(d => d.Customer).WithMany(p => p.BookRequestCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookRequests_Users");

            builder.HasOne(d => d.FreeBook).WithMany(p => p.BookRequests)
                .HasForeignKey(d => d.FreeBookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookRequests_FreeBooks");

            builder.HasOne(d => d.ReviewedByEmployee).WithMany(p => p.BookRequestReviewedByEmployees)
                .HasForeignKey(d => d.ReviewedByEmployeeId)
                .HasConstraintName("FK_BookRequests_Employee");
        }
    }
}
