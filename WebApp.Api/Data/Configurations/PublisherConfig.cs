using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Api.Entities;

namespace WebApp.Api.Data.Configurations
{
    public class PublisherConfig : IEntityTypeConfiguration<Publisher>
    {
        public void Configure(EntityTypeBuilder<Publisher> builder)
        {
            builder.HasKey(e => e.PublisherId).HasName("PK__Publishe__4C657FABF22285D1");

            builder.Property(e => e.Address).HasMaxLength(300);
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.PhoneNumber).HasMaxLength(50);
            builder.Property(e => e.PublisherName).HasMaxLength(150);
        }
    }
}
