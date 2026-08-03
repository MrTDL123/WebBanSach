using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class FreeBookConfig : IEntityTypeConfiguration<FreeBook>
    {
        public void Configure(EntityTypeBuilder<FreeBook> builder)
        {
            builder.HasKey(e => e.FreeBookId).HasName("PK__FreeBook__57A71AF3C46F6C8B");

            builder.Property(e => e.AuthorName).HasMaxLength(150);
            builder.Property(e => e.Condition)
                .HasMaxLength(50)
                .HasDefaultValue("Good");
            builder.Property(e => e.CoverImageUrl).HasMaxLength(500);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.Source).HasMaxLength(100);
            builder.Property(e => e.Title).HasMaxLength(255);
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");
        }
    }
}
