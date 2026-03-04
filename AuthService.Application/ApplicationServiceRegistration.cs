using AuthService.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<RegisterUseCase>();
            services.AddScoped<LoginUseCase>();
            return services;
        }
    }
}
