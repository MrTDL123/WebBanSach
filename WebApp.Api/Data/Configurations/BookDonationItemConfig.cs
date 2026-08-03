using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class BookDonationItemConfig : IEntityTypeConfiguration<BookDonationItem>
    {
        public void Configure(EntityTypeBuilder<BookDonationItem> builder)
        {
            builder.HasKey(e => e.ItemId).HasName("PK__BookDona__727E838B0A79E232");

            builder.Property(e => e.AuthorName).HasMaxLength(150);
            builder.Property(e => e.BookTitle).HasMaxLength(255);
            builder.Property(e => e.Condition).HasMaxLength(50);
            builder.Property(e => e.Note).HasMaxLength(300);
            builder.Property(e => e.Quantity).HasDefaultValue(1);

            builder.HasOne(d => d.Donation).WithMany(p => p.BookDonationItems)
                .HasForeignKey(d => d.DonationId)
                .HasConstraintName("FK_DonationItems_Donations");

            builder.HasOne(d => d.LinkedFreeBook).WithMany(p => p.BookDonationItems)
                .HasForeignKey(d => d.LinkedFreeBookId)
                .HasConstraintName("FK_DonationItems_FreeBook");
        }
    }
}
