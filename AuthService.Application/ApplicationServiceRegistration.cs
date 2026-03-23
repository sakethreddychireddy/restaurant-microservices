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
            services.AddScoped<OAuthLoginUseCase>();
            services.AddScoped<ProfileUseCase>();
            services.AddScoped<OtpUseCase>();
            services.AddScoped<RefreshTokenUseCase>();
            services.AddScoped<LogoutUseCase>();
            return services;
        }
    }
}
