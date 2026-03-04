using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Http;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Persistence.Repositories;

namespace OrderService.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            services.AddDbContext<OrderDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IOrderRepository, OrderRepository>();
            // HTTP clients
            services.AddHttpClient<IMenuServiceClient, MenuServiceHttpClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["MenuService:BaseUrl"]!);
            })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler();
                    // Optionally configure the handler (e.g., for SSL, proxies, etc.)
                    handler.ServerCertificateCustomValidationCallback
                    = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    return handler;
                });

            // RabbitMQ publisher
            services.AddSingleton<IEventPublisher, RabbitMqPublisher>();

            return services;
        }
    }
}
