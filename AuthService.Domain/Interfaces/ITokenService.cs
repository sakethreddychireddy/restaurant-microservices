using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
