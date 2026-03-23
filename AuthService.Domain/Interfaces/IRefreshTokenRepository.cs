using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(
            string token, CancellationToken ct = default);
        Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(
            Guid userId, CancellationToken ct = default);
        Task AddAsync(
            RefreshToken token, CancellationToken ct = default);
        Task UpdateAsync(
            RefreshToken token, CancellationToken ct = default);
        Task RevokeAllByUserIdAsync(
            Guid userId, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}