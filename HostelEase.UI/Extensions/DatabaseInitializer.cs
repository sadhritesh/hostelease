using HostelEase.Infrastructure.Data;
using HostelEase.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace HostelEase.UI.Extensions
{
    public static class DatabaseInitializer
    {
        public static async Task InitilizeAsync(this WebApplication app) 
        { 
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                //await context.Database.MigrateAsync();

                if (app.Environment.IsDevelopment())
                {
                    await DbSeeder.Seed(context);
                }
            }
            catch (Exception ex) {
                logger.LogCritical(ex,"Database initialization failed.");

                throw;
            }
        }
    }
}
