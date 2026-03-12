using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Auth;
using AuthService.Infrastructure.Identity;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
            public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
            {
                services.AddDbContext<AuthDbContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
                services.AddScoped<ITokenService, JwtTokenService>();
                services.AddMemoryCache();
                services.AddSingleton<IOAuthCodeStore, InMemoryOAuthCodeStore>();
                return services;
            }
    }
}
