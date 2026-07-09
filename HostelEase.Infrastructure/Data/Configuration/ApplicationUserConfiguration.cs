using HostelEase.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace HostelEase.Infrastructure.Data.Configuration
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("ApplicationUser");
            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.LastName)
                .HasMaxLength(256);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedOn)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.UpdatedOn)
                .ValueGeneratedOnUpdate();
        }
    }
}
