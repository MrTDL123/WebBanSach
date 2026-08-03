using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class ReturnRequestConfig : IEntityTypeConfiguration<ReturnRequest>
    {
        public void Configure(EntityTypeBuilder<ReturnRequest> builder)
        {
            builder.HasKey(e => e.ReturnRequestId).HasName("PK__ReturnRe__0CCD25997619BD5C");

            builder.HasIndex(e => e.OrderId, "UQ__ReturnRe__C3905BCE83CDF7C2").IsUnique();

            builder.Property(e => e.BankAccountHolder).HasMaxLength(150);
            builder.Property(e => e.BankAccountNumber).HasMaxLength(50);
            builder.Property(e => e.BankName).HasMaxLength(100);
            builder.Property(e => e.CustomerId).HasMaxLength(450);

            builder.Property(e => e.EmployeeNotes).HasMaxLength(500);
            builder.Property(e => e.HandledByEmployeeId).HasMaxLength(450);
            builder.Property(e => e.ProofImageUrl).HasMaxLength(500);
            builder.Property(e => e.Reason).HasMaxLength(1000);
            builder.Property(e => e.RefundAmount).HasColumnType("decimal(18, 2)");
            builder.Property(e => e.RequestedAt).HasDefaultValueSql("(getutcdate())");

            builder.HasOne(d => d.Customer).WithMany(p => p.ReturnRequestCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReturnRequests_Customer");

            builder.HasOne(d => d.HandledByEmployee).WithMany(p => p.ReturnRequestHandledByEmployees)
                .HasForeignKey(d => d.HandledByEmployeeId)
                .HasConstraintName("FK_ReturnRequests_Employee");

            builder.HasOne(d => d.Order).WithOne(p => p.ReturnRequest)
                .HasForeignKey<ReturnRequest>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReturnRequests_Orders");
        }
    }
}
