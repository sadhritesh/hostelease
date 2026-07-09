using Microsoft.AspNetCore.Mvc;

namespace HostelEase.UI.Extensions
{
    public static class MvcExtensions
    {
        public static IServiceCollection AddMvcServices(this IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            return services;
        }
    }
}
