using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces
{
    public interface INotificationLogRepository
    {
        Task AddAsync(NotificationLog log, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}