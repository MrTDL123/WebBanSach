using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class SupportMessageConfig : IEntityTypeConfiguration<SupportMessage>
    {
        public void Configure(EntityTypeBuilder<SupportMessage> builder)
        {
            builder.HasKey(e => e.MessageId).HasName("PK__SupportM__C87C0C9CBDC62CAC");

            builder.Property(e => e.SenderId).HasMaxLength(450);
            builder.Property(e => e.SentAt).HasDefaultValueSql("(getutcdate())");

            builder.HasOne(d => d.Sender).WithMany(p => p.SupportMessages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupportMessages_Users");

            builder.HasOne(d => d.Ticket).WithMany(p => p.SupportMessages)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK_SupportMessages_Tickets");
        }
    }
}
