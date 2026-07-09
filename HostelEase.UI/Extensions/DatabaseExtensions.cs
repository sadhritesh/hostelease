using HostelEase.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HostelEase.UI.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(Options =>
            {
                Options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                   sqlOptions =>
                   {
                       sqlOptions.EnableRetryOnFailure(
                           maxRetryCount: 3,
                           maxRetryDelay: TimeSpan.FromSeconds(10),
                           errorNumbersToAdd: null
                           );
                       sqlOptions.CommandTimeout(30);
                   });
            });

            return services;
        }
    }
}
