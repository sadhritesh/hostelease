
namespace HostelEase.Infrastructure.Data.Seed
{
    public class DbSeeder
    {
        public static async Task Seed(ApplicationDbContext context)
        {
            await HostelSeeder.SeedHostels(context);
        }
    }
}
