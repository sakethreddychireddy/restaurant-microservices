using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Infrastructure.Auth
{
    public class JwtTokenService(IConfiguration config) : ITokenService
    {
        public string GenerateToken(User user)
        {
            var section = config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(section["SecretKey"]!));
            var creds = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email,          user.Email),
                new Claim(ClaimTypes.Name,           user.Name),
                new Claim(ClaimTypes.Role,           user.Role),
            };

            var token = new JwtSecurityToken(
                issuer: section["Issuer"],
                audience: section["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(section["ExpiryMinutes"] ?? "60")),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Guid? ValidateToken(string token)
        {
            try
            {
                var section = config.GetSection("JwtSettings");
                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(section["SecretKey"]!));

                var handler = new JwtSecurityTokenHandler();
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = section["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = section["Audience"],
                    ValidateLifetime = false, // allow expired for refresh
                    ClockSkew = TimeSpan.Zero
                };

                var principal = handler.ValidateToken(
                    token, parameters, out _);

                var userId = principal.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

                return userId is null ? null : Guid.Parse(userId);
            }
            catch
            {
                return null;
            }
        }
    }
}