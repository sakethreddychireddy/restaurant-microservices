namespace AuthService.Domain.Interfaces
{
    public interface IOtpStore
    {
        string GenerateOtp(string email);
        bool VerifyOtp(string email, string otp);
    }
}