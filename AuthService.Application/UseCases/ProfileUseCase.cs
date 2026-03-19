using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.UseCases
{
    public class ProfileUseCase(
        IUserRepository userRepository,
        IUserAddressRepository addressRepository,
        ILogger<ProfileUseCase> logger)
    {
        // ── Get profile ────────────────────────────────────────
        public async Task<ProfileResponse?> GetProfileAsync(
            Guid userId, CancellationToken ct = default)
        {
            var user = await userRepository.GetByIdAsync(userId, ct);
            if (user is null) return null;

            return new ProfileResponse(
                user.Id, user.Name, user.Email,
                user.Phone, user.Role, user.IsOAuthUser());
        }

        // ── Update profile ─────────────────────────────────────
        public async Task<ProfileResponse?> UpdateProfileAsync(
            Guid userId, UpdateProfileRequest request,
            CancellationToken ct = default)
        {
            var user = await userRepository.GetByIdAsync(userId, ct);
            if (user is null) return null;

            user.UpdateProfile(request.Name, request.Phone);
            await userRepository.UpdateAsync(user, ct);
            await userRepository.SaveChangesAsync(ct);

            logger.LogInformation(
                "Profile updated for user {UserId}", userId);

            return new ProfileResponse(
                user.Id, user.Name, user.Email,
                user.Phone, user.Role, user.IsOAuthUser());
        }

        // ── Get addresses ──────────────────────────────────────
        public async Task<IEnumerable<AddressResponse>> GetAddressesAsync(
            Guid userId, CancellationToken ct = default)
        {
            var addresses = await addressRepository
                .GetByUserIdAsync(userId, ct);

            return addresses.Select(a => new AddressResponse(
                a.Id, a.Label, a.FullAddress,
                a.IsDefault, a.CreatedAt));
        }

        // ── Add address ────────────────────────────────────────
        public async Task<AddressResponse> AddAddressAsync(
            Guid userId, AddressRequest request,
            CancellationToken ct = default)
        {
            // if new address is default, unset others
            if (request.IsDefault)
                await UnsetDefaultAddressesAsync(userId, ct);

            var address = UserAddress.Create(
                userId, request.Label,
                request.FullAddress, request.IsDefault);

            await addressRepository.AddAsync(address, ct);
            await addressRepository.SaveChangesAsync(ct);

            logger.LogInformation(
                "Address added for user {UserId}", userId);

            return new AddressResponse(
                address.Id, address.Label,
                address.FullAddress, address.IsDefault,
                address.CreatedAt);
        }

        // ── Update address ─────────────────────────────────────
        public async Task<AddressResponse?> UpdateAddressAsync(
            Guid userId, Guid addressId,
            AddressRequest request,
            CancellationToken ct = default)
        {
            var address = await addressRepository
                .GetByIdAsync(addressId, ct);

            if (address is null || address.UserId != userId)
                return null;

            if (request.IsDefault)
                await UnsetDefaultAddressesAsync(userId, ct);

            address.Update(request.Label, request.FullAddress);
            address.SetDefault(request.IsDefault);

            await addressRepository.UpdateAsync(address, ct);
            await addressRepository.SaveChangesAsync(ct);

            return new AddressResponse(
                address.Id, address.Label,
                address.FullAddress, address.IsDefault,
                address.CreatedAt);
        }

        // ── Delete address ─────────────────────────────────────
        public async Task<bool> DeleteAddressAsync(
            Guid userId, Guid addressId,
            CancellationToken ct = default)
        {
            var address = await addressRepository
                .GetByIdAsync(addressId, ct);

            if (address is null || address.UserId != userId)
                return false;

            await addressRepository.DeleteAsync(addressId, ct);
            await addressRepository.SaveChangesAsync(ct);

            return true;
        }

        // ── Helper ─────────────────────────────────────────────
        private async Task UnsetDefaultAddressesAsync(
            Guid userId, CancellationToken ct)
        {
            var addresses = await addressRepository
                .GetByUserIdAsync(userId, ct);

            foreach (var a in addresses.Where(a => a.IsDefault))
            {
                a.SetDefault(false);
                await addressRepository.UpdateAsync(a, ct);
            }
            await addressRepository.SaveChangesAsync(ct);
        }
    }
}