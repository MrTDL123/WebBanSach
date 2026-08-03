using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class BookDonationConfig : IEntityTypeConfiguration<BookDonation>
    {
        public void Configure(EntityTypeBuilder<BookDonation> builder)
        {
            builder.HasKey(e => e.DonationId).HasName("PK__BookDona__C5082EFBEE767686");

            builder.HasIndex(e => e.DonationCode, "UQ__BookDona__0B61C73C9CAD1C83").IsUnique();

            builder.Property(e => e.DonatedAt).HasDefaultValueSql("(getutcdate())");
            builder.Property(e => e.DonationCode).HasMaxLength(20);
            builder.Property(e => e.DonorEmail).HasMaxLength(256);
            builder.Property(e => e.DonorId).HasMaxLength(450);
            builder.Property(e => e.DonorMessage).HasMaxLength(1000);
            builder.Property(e => e.DonorName).HasMaxLength(150);
            builder.Property(e => e.DonorPhone).HasMaxLength(20);
            builder.Property(e => e.PickupAddress).HasMaxLength(500);
            builder.Property(e => e.PickupTrackingNumber).HasMaxLength(100);
            builder.Property(e => e.ReceivedByEmployeeId).HasMaxLength(450);
            builder.Property(e => e.RejectedReason).HasMaxLength(500);
            builder.Property(e => e.ReviewedByEmployeeId).HasMaxLength(450);
            builder.Property(e => e.WarehouseNote).HasMaxLength(500);

            builder.HasOne(d => d.Donor).WithMany(p => p.BookDonationDonors)
                .HasForeignKey(d => d.DonorId)
                .HasConstraintName("FK_BookDonations_Donor");

            builder.HasOne(d => d.Provider).WithMany(p => p.BookDonations)
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("FK_BookDonations_Provider");

            builder.HasOne(d => d.ReceivedByEmployee).WithMany(p => p.BookDonationReceivedByEmployees)
                .HasForeignKey(d => d.ReceivedByEmployeeId)
                .HasConstraintName("FK_BookDonations_Warehouse");

            builder.HasOne(d => d.ReviewedByEmployee).WithMany(p => p.BookDonationReviewedByEmployees)
                .HasForeignKey(d => d.ReviewedByEmployeeId)
                .HasConstraintName("FK_BookDonations_Employee");
        }
    }
}
