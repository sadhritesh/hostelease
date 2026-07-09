using HostelEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace HostelEase.Infrastructure.Data.Configuration
{
    public class HostelConfiguration : IEntityTypeConfiguration<Hostel>
    {
        public void Configure(EntityTypeBuilder<Hostel> builder)
        {
            builder.ToTable("Hostels");

            builder.HasKey(x => x.HostelId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

        }
    }
}
