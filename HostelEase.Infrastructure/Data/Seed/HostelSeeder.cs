using HostelEase.Domain.Entities;

namespace HostelEase.Infrastructure.Data.Seed
{
    public static class HostelSeeder
    {
        public static async Task SeedHostels(ApplicationDbContext context)
        {

            // Create default data
            var now = DateTime.UtcNow;
            var hostels = new List<Hostel>
            {
                new Hostel
                {
                    HostelId = Guid.NewGuid(),
                    Name = "Maple Residency",
                    Address = "12 Maple Street, Block A",
                    IsActive = true,
                    CreatedAt = now
                },
                new Hostel
                {
                    HostelId = Guid.NewGuid(),
                    Name = "Oakwood Hostel",
                    Address = "98 Oakwood Avenue, Sector 4",
                    IsActive = true,
                    CreatedAt = now
                },
                new Hostel
                {
                    HostelId = Guid.NewGuid(),
                    Name = "Pine View Hostel",
                    Address = "4 Pine View Lane, North Campus",
                    IsActive = true,
                    CreatedAt = now
                },
                new Hostel
                {
                    HostelId = Guid.NewGuid(),
                    Name = "Riverdale Hostel",
                    Address = "77 Riverdale Road, Riverside",
                    IsActive = false,
                    CreatedAt = now
                },

                // 6 additional records requested
                new Hostel
                {
                    HostelId = Guid.NewGuid(),
                    Name = "Willow Heights",
                    Address = "5 Willow Terrace, Downtown",
                    IsActive = true,
                    CreatedAt = now
                },
                new Hostel
                {
                    HostelId = Guid.NewGuid(),
                    Name = "Cedar Court",
                    Address = "21 Cedar Court, West Park",
                    IsActive = true,
                    CreatedAt = now
                },
                new Hostel
                {
                    HostelId = Guid.NewGuid(),
                    Name = "Elm Retreat",
                    Address = "3 Elm Street, Old Town",
                    IsActive = true,
                    CreatedAt = now
                },
                new Hostel
                {
                    HostelId = Guid.NewGuid(),
                    Name = "Birch Haven",
                    Address = "44 Birch Lane, Greenfield",
                    IsActive = true,
                    CreatedAt = now
                },
                new Hostel
                {
                    HostelId = Guid.NewGuid(),
                    Name = "Harbor View Hostel",
                    Address = "8 Harbor Road, Seafront",
                    IsActive = false,
                    CreatedAt = now
                },
                new Hostel
                {
                    HostelId = Guid.NewGuid(),
                    Name = "Lakeside Inn",
                    Address = "16 Lakeside Drive, North Bay",
                    IsActive = true,
                    CreatedAt = now
                }
            };

            // add to context
            await context.Hostels.AddRangeAsync(hostels);

            // save to db
            await context.SaveChangesAsync(CancellationToken.None);
        }
    }
}
