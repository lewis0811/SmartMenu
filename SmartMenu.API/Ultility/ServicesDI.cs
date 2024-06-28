using SmartMenu.DAO.Implementation;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using SmartMenu.Service.Services;

namespace SmartMenu.API.Ultility
{
    public static class ServicesDI 
    {
        public static IServiceCollection AddDIServices(this IServiceCollection services)
        {
            // Repositories DI
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services DI
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
