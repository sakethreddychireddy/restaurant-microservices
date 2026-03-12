using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.UseCases
{
    public sealed class OAuthLoginUseCase(
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<OAuthLoginUseCase> logger)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<OAuthLoginUseCase> _logger = logger;

        public async Task<AuthResponse> ExecuteAsync(
            string email,
            string name,
            string provider,
            string providerId,
            CancellationToken ct = default)
        {
            _logger.LogInformation(
                "OAuth login attempt via {Provider} for {Email}", provider, email);

            // check if user already exists
            var user = await _userRepository.GetByEmailAsync(email, ct);

            if (user is null)
            {
                // first time OAuth login — create new user
                _logger.LogInformation(
                    "Creating new OAuth user for {Email} via {Provider}", email, provider);
                user = User.CreateOAuth(name, email, provider, providerId);
                await _userRepository.AddAsync(user, ct);
                await _userRepository.SaveChangesAsync(ct);
            }
            else
            {
                _logger.LogInformation(
                    "Existing user {Email} logged in via {Provider}", email, provider);
            }

            var token = _tokenService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                UserId = user.Id
            };
        }
    }
}
