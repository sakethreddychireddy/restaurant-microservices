using MenuService.Domain.Entities;

namespace MenuService.Application.Interfaces
{
    public interface IMenuItemRepository
    {
        Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<MenuItem>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<MenuItem>> GetByCategoryAsync(string category, CancellationToken ct = default);
        Task<IEnumerable<MenuItem>> GetAvailableAsync(CancellationToken ct = default);
        Task AddAsync(MenuItem item, CancellationToken ct = default);
        Task UpdateAsync(MenuItem item, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    }
}
