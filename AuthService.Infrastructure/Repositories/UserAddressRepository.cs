using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories
{
    public class UserAddressRepository : IUserAddressRepository
    {
        private readonly AuthDbContext _context;

        public UserAddressRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserAddress>> GetByUserIdAsync(
            Guid userId, CancellationToken ct = default)
            => await _context.UsersAddresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync(ct);

        public async Task<UserAddress?> GetByIdAsync(
            Guid id, CancellationToken ct = default)
            => await _context.UsersAddresses
                .FirstOrDefaultAsync(a => a.Id == id, ct);

        public async Task AddAsync(
            UserAddress address, CancellationToken ct = default)
            => await _context.UsersAddresses.AddAsync(address, ct);

        public async Task UpdateAsync(
            UserAddress address, CancellationToken ct = default)
            => _context.UsersAddresses.Update(address);

        public async Task DeleteAsync(
            Guid id, CancellationToken ct = default)
        {
            var address = await GetByIdAsync(id, ct);
            if (address is not null)
                _context.UsersAddresses.Remove(address);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);
    }
}