using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories
{
    public sealed class UserRepository(AuthDbContext context) : IUserRepository
    {
        private readonly AuthDbContext _context = context;
        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
            =>await _context.Users.FindAsync([id], ct);
        public async Task<User?> GetByEmailAsync(string Email, CancellationToken ct = default)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == Email, ct);
        public async Task<bool> ExistsByEmailAsync(string Email, CancellationToken ct = default)
            => await _context.Users.AnyAsync(u => u.Email == Email, ct);
        public async Task AddAsync(User user, CancellationToken ct = default)
            => await _context.Users.AddAsync(user, ct);
        public async Task SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);
    }
}
