using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace AuthService.Infrastructure.Auth
{
    public class InMemoryOtpStore : IOtpStore
    {
        private readonly IMemoryCache _cache;

        public InMemoryOtpStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerateOtp(string email)
        {
            // generate 6 digit OTP
            var otp = Random.Shared.Next(100000, 999999).ToString();

            // store with 10 minute expiry
            _cache.Set(
                CacheKey(email), otp,
                TimeSpan.FromMinutes(10));

            return otp;
        }

        public bool VerifyOtp(string email, string otp)
        {
            var key = CacheKey(email);

            if (!_cache.TryGetValue(key, out string? stored))
                return false;

            if (stored != otp)
                return false;

            // remove after use — one time only
            _cache.Remove(key);
            return true;
        }

        private static string CacheKey(string email)
            => $"otp:{email.ToLowerInvariant()}";
    }
}