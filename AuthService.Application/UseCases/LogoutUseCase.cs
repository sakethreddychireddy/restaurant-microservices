using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.UseCases
{
    public class LogoutUseCase(
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<LogoutUseCase> logger)
    {
        public async Task ExecuteAsync(
            string refreshToken,
            CancellationToken ct = default)
        {
            var token = await refreshTokenRepository
                .GetByTokenAsync(refreshToken, ct);

            if (token is null) return;

            token.Revoke();
            await refreshTokenRepository.UpdateAsync(token, ct);
            await refreshTokenRepository.SaveChangesAsync(ct);

            logger.LogInformation(
                "User {UserId} logged out", token.UserId);
        }
    }
}