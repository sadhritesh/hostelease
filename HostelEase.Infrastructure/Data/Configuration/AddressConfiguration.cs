using HostelEase.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelEase.Infrastructure.Data.Configuration
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.State)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.Country)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.CreatedOn)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.UpdatedOn)
                .ValueGeneratedOnUpdate();

            builder.HasOne(u => u.User)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
