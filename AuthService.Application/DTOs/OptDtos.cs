namespace AuthService.Application.DTOs
{
    public record SendOtpRequest(string Email);

    public record VerifyOtpRequest(
        string Email,
        string Otp,
        string? Name = null);

    public record SendOtpResponse(
        bool IsNewUser,
        string Message);
}