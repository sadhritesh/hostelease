using HostelEase.Infrastructure.Data;
using HostelEase.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace HostelEase.UI.Extensions
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services) 
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            });

            return services;
        }
    }
}
