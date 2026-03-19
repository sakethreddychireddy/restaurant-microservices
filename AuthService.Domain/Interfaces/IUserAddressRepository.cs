using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces
{
    public interface IUserAddressRepository
    {
        Task<IEnumerable<UserAddress>> GetByUserIdAsync(
            Guid userId, CancellationToken ct = default);
        Task<UserAddress?> GetByIdAsync(
            Guid id, CancellationToken ct = default);
        Task AddAsync(
            UserAddress address, CancellationToken ct = default);
        Task UpdateAsync(
            UserAddress address, CancellationToken ct = default);
        Task DeleteAsync(
            Guid id, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}