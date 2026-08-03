using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class AuthorConfig : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC348CDDD5A0");

            builder.Property(e => e.AuthorName).HasMaxLength(150);
            builder.Property(e => e.IsActive).HasDefaultValue(true);
        }
    }
}
