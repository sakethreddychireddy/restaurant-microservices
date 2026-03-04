using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(Order order, CancellationToken ct = default);
        Task UpdateAsync(Order order, CancellationToken ct = default);
    }
}
