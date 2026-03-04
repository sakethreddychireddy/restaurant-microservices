using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context) => _context = context;

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id, ct);

        public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
            => await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(ct);

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default)
            => await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(ct);

        public async Task AddAsync(Order order, CancellationToken ct = default)
        {
            await _context.Orders.AddAsync(order, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Order order, CancellationToken ct = default)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync(ct);
        }
    }
}
