namespace AuthService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string Role { get; private set; } = "Customer";
        public string? Phone { get; private set; }
        public string? OAuthProvider { get; private set; }
        public string? OAuthProviderId { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private readonly List<UserAddress> _addresses = new();
        public IReadOnlyCollection<UserAddress> Addresses =>
            _addresses.AsReadOnly();

        private User() { }

        public static User Create(
            string name, string email,
            string passwordHash, string role = "Customer")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
            return new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                Role = role,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static User CreateOAuth(
            string name, string email,
            string provider, string providerId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            return new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                PasswordHash = string.Empty,
                Role = "Customer",
                OAuthProvider = provider,
                OAuthProviderId = providerId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateProfile(string? name, string? phone) 
        {
            if (!string.IsNullOrWhiteSpace(name)) Name = name;
            if (phone is not null) Phone = phone;
        }

        public bool IsAdmin() => Role.Equals("Admin");
        public bool IsOAuthUser() => !string.IsNullOrEmpty(OAuthProvider);
    }
}
