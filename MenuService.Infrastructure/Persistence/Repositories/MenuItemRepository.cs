using MenuService.Application.Interfaces;
using MenuService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MenuService.Infrastructure.Persistence.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly MenuDbContext _context;
        public MenuItemRepository(MenuDbContext context)
        {
            _context = context;
        }
        public async Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.MenuItems.FindAsync([id], ct);

        public async Task<IEnumerable<MenuItem>> GetAllAsync(CancellationToken ct = default)
            => await _context.MenuItems.AsNoTracking().ToListAsync(ct);

        public async Task<IEnumerable<MenuItem>> GetByCategoryAsync(string category, CancellationToken ct = default)
            => await _context.MenuItems.AsNoTracking()
                .Where(x => x.Category == category)
                .ToListAsync(ct);

        public async Task<IEnumerable<MenuItem>> GetAvailableAsync(CancellationToken ct = default)
            => await _context.MenuItems.AsNoTracking()
                .Where(x => x.IsAvailable)
                .ToListAsync(ct);

        public async Task AddAsync(MenuItem item, CancellationToken ct = default)
        {
            await _context.MenuItems.AddAsync(item, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(MenuItem item, CancellationToken ct = default)
        {
            _context.MenuItems.Update(item);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var item = await GetByIdAsync(id, ct);
            if (item is not null)
            {
                _context.MenuItems.Remove(item);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
            => await _context.MenuItems.AnyAsync(x => x.Id == id, ct);
    }
}
