namespace AuthService.Domain.Interfaces
{
    public interface IOAuthCodeStore
    {
        string GenerateCode(AuthCodePayload payload);
        AuthCodePayload? ConsumeCode(string code);
    }

    public class AuthCodePayload
    {
        public string Token { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
