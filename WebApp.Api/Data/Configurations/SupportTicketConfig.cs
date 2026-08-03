using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class SupportTicketConfig : IEntityTypeConfiguration<SupportTicket>
    {
        public void Configure(EntityTypeBuilder<SupportTicket> builder)
        {
            builder.HasKey(e => e.TicketId).HasName("PK__SupportT__712CC607D258B072");

            builder.Property(e => e.AssignedEmployeeId).HasMaxLength(450);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            builder.Property(e => e.CustomerId).HasMaxLength(450);
            builder.Property(e => e.Subject).HasMaxLength(255);
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");

            builder.HasOne(d => d.AssignedEmployee).WithMany(p => p.SupportTicketAssignedEmployees)
                .HasForeignKey(d => d.AssignedEmployeeId)
                .HasConstraintName("FK_SupportTickets_Employee");

            builder.HasOne(d => d.Customer).WithMany(p => p.SupportTicketCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupportTickets_Customer");
        }
    }
}
