using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.UseCases
{
    public class RefreshTokenUseCase(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<RefreshTokenUseCase> logger)
    {
        public async Task<AuthResponse?> ExecuteAsync(
            string refreshToken,
            CancellationToken ct = default)
        {
            var existing = await refreshTokenRepository
                .GetByTokenAsync(refreshToken, ct);

            if (existing is null || !existing.IsActive)
            {
                logger.LogWarning(
                    "Invalid or expired refresh token used");
                return null;
            }

            var user = await userRepository
                .GetByIdAsync(existing.UserId, ct);

            if (user is null) return null;

            // revoke old refresh token
            existing.Revoke();
            await refreshTokenRepository.UpdateAsync(existing, ct);

            // issue new tokens
            var newJwt = tokenService.GenerateToken(user);
            var newRefreshToken = RefreshToken.Create(user.Id, 30);

            await refreshTokenRepository.AddAsync(newRefreshToken, ct);
            await refreshTokenRepository.SaveChangesAsync(ct);

            logger.LogInformation(
                "Refresh token rotated for user {UserId}", user.Id);

            return new AuthResponse
            {
                Token = newJwt,
                RefreshToken = newRefreshToken.Token,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                UserId = user.Id
            };
        }
    }
}