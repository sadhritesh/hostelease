using HostelEase.Application.Common.Interfaces;
using HostelEase.Application.Interfaces.RepositoryContracts;
using HostelEase.Application.Interfaces.ServiceContracts;
using HostelEase.Application.Services;
using HostelEase.Infrastructure.Repositories;
using HostelEase.Infrastructure.Services;

namespace HostelEase.UI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services) 
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IHostelRepository, HostelsRepository>();
            services.AddScoped<IHostelService, HostelService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
