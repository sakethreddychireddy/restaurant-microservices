using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories
{
    public class RefreshTokenRepository(AuthDbContext context)
         : IRefreshTokenRepository
    {
        public async Task<RefreshToken?> GetByTokenAsync(
            string token, CancellationToken ct = default)
            => await context.RefreshTokens
                .Include(r => r.UserId)
                .FirstOrDefaultAsync(r => r.Token == token, ct);

        public async Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(
            Guid userId, CancellationToken ct = default)
            => await context.RefreshTokens
                .Where(r => r.UserId == userId &&
                            !r.IsRevoked &&
                            r.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(ct);

        public async Task AddAsync(
            RefreshToken token, CancellationToken ct = default)
            => await context.RefreshTokens.AddAsync(token, ct);

        public async Task UpdateAsync(
            RefreshToken token, CancellationToken ct = default)
            => context.RefreshTokens.Update(token);

        public async Task RevokeAllByUserIdAsync(
            Guid userId, CancellationToken ct = default)
        {
            var tokens = await context.RefreshTokens
                .Where(r => r.UserId == userId && !r.IsRevoked)
                .ToListAsync(ct);
            foreach (var t in tokens) t.Revoke();
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
            => await context.SaveChangesAsync(ct);
    }
}
