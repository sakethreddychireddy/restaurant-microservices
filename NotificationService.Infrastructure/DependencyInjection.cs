using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Interfaces;
using NotificationService.Infrastructure.Email;
using NotificationService.Infrastructure.Messaging;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            // Email
            services.AddScoped<IEmailService, GmailEmailService>();

            // Notification log — in memory for now
            services.AddScoped<INotificationLogRepository,
                InMemoryNotificationLogRepository>();

            // RabbitMQ consumer as background service
            services.AddHostedService<RabbitMqConsumer>();

            return services;
        }
    }
}