using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.UseCases
{
    public sealed class RegisterUseCase(IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ILogger<RegisterUseCase> logger)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<RegisterUseCase> _logger = logger;

        public async Task<AuthResponse?> ExecuteAsync(RegisterRequest request, CancellationToken ct = default)
        {
            _logger.LogInformation("Starting registration for email: {Email}", request.Email);
            if(await _userRepository.ExistsByEmailAsync(request.Email, ct))
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists", request.Email);
                return null; // Or throw an exception if you prefer
            }
            var passwordHash = _passwordHasher.Hash(request.Password);
            var user = User.Create(request.Name, request.Email, passwordHash);
            await _userRepository.AddAsync(user, ct);
            await _userRepository.SaveChangesAsync(ct);
            _logger.LogInformation("User registered successfully with email: {Email}", request.Email);

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
