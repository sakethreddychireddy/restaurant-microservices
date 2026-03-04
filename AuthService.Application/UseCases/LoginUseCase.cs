using AuthService.Application.DTOs;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.UseCases
{
    public sealed class LoginUseCase(IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<LoginUseCase> logger)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<LoginUseCase> _logger = logger;

        public async Task<AuthResponse?> ExecuteAsync(LoginRequest request, CancellationToken ct = default)
        {
            _logger.LogInformation("Starting login for email: {Email}", request.Email);
            var user = await _userRepository.GetByEmailAsync(request.Email, ct);
            if(user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed for email: {Email}", request.Email);
                return null;
            }
            var token = _tokenService.GenerateToken(user);
            _logger.LogInformation("Login successful for email: {Email}", request.Email);
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
