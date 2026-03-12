using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace AuthService.Infrastructure.Auth
{
    public class InMemoryOAuthCodeStore : IOAuthCodeStore
    {
        private readonly IMemoryCache _cache;

        public InMemoryOAuthCodeStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerateCode(AuthCodePayload payload)
        {
            // random 32-byte URL-safe code
            var code = Convert.ToBase64String(
                System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');

            // expires in 60 seconds — one time use
            _cache.Set(
                $"oauth_code:{code}",
                payload,
                TimeSpan.FromSeconds(60));

            return code;
        }

        public AuthCodePayload? ConsumeCode(string code)
        {
            var key = $"oauth_code:{code}";

            if (_cache.TryGetValue(key, out AuthCodePayload? payload))
            {
                _cache.Remove(key); // ← delete after use (one-time)
                return payload;
            }

            return null; // expired or invalid
        }
    }
}
