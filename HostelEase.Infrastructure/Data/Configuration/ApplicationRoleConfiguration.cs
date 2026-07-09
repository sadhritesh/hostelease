using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HostelEase.Infrastructure.Identity;

namespace HostelEase.Infrastructure.Data.Configuration
{
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            var roles = new[]
            {
                new ApplicationRole
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "Full access to the system",
                    IsActive = true,
                    CreatedOn = new DateTime(2026, 6, 07, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedOn = null,
                    ConcurrencyStamp = "a1e5f7d0-0000-4000-8000-000000000001"
                },
                new ApplicationRole
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    Description = "Manage hostels and assignments",
                    IsActive = true,
                    CreatedOn = new DateTime(2026, 6, 07, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedOn = null,
                },
                new ApplicationRole
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Student",
                    NormalizedName = "STUDENT",
                    Description = "Regular user with limited permissions",
                    IsActive = true,
                    CreatedOn = new DateTime(2026, 6, 07, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedOn = null
                }
            };

            builder.ToTable("ApplicationRole");
            builder.HasData(roles);
        }
    }
}