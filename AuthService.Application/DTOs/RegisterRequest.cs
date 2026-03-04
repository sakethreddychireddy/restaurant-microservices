using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public record RegisterRequest
    {
        [Required]
        public string Name { get; init; } = string.Empty;
        [Required,EmailAddress]
        public string Email { get; init; } = string.Empty;
        [Required,MinLength(6)]
        public string Password { get; init; } = string.Empty;
    }
}
