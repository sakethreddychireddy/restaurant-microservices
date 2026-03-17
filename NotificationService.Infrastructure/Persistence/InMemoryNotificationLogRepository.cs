using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace NotificationService.Infrastructure.Persistence
{
    public class InMemoryNotificationLogRepository : INotificationLogRepository
    {
        private readonly ILogger<InMemoryNotificationLogRepository> _logger;

        public InMemoryNotificationLogRepository(
            ILogger<InMemoryNotificationLogRepository> logger)
        {
            _logger = logger;
        }

        public Task AddAsync(NotificationLog log, CancellationToken ct = default)
        {
            _logger.LogInformation(
                "Notification log — Email: {Email} | Event: {Event} | Success: {Success}",
                log.RecipientEmail, log.EventType, log.IsSuccess);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
            => Task.CompletedTask;
    }
}