namespace AuthService.Domain.Entities
{
    public class UserAddress
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Label { get; private set; } = string.Empty;
        public string FullAddress { get; private set; } = string.Empty;
        public bool IsDefault { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private UserAddress() { }

        public static UserAddress Create(
            Guid userId,
            string label,
            string fullAddress,
            bool isDefault = false)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(label);
            ArgumentException.ThrowIfNullOrWhiteSpace(fullAddress);

            return new UserAddress
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Label = label,
                FullAddress = fullAddress,
                IsDefault = isDefault,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(string? label, string? fullAddress)
        {
            if (!string.IsNullOrWhiteSpace(label)) Label = label;
            if (!string.IsNullOrWhiteSpace(fullAddress)) FullAddress = fullAddress;
        }

        public void SetDefault(bool isDefault)
            => IsDefault = isDefault;
    }
}