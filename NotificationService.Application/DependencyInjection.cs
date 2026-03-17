using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Handlers;

namespace NotificationService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<OrderPlacedHandler>();
            services.AddScoped<OrderStatusChangedHandler>();
            services.AddScoped<OrderCancelledHandler>();
            return services;
        }
    }
}