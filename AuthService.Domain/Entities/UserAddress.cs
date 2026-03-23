namespace AuthService.Domain.Entities
{
    public class UserAddress
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Label { get; private set; } = string.Empty;
        public string AddressLine1 { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string State { get; private set; } = string.Empty;
        public string ZipCode { get; private set; } = string.Empty;
        public string Country { get; private set; } = string.Empty;
        public bool IsDefault { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // computed — not stored in DB
        public string FullAddress =>
            $"{AddressLine1}, {City}, {State} {ZipCode}, {Country}";

        public string ShortAddress =>
            $"{AddressLine1}, {City}";

        private UserAddress() { }

        public static UserAddress Create(
            Guid userId,
            string label,
            string addressLine1,
            string city,
            string state,
            string zipCode,
            string country,
            bool isDefault = false)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(addressLine1);
            ArgumentException.ThrowIfNullOrWhiteSpace(city);
            ArgumentException.ThrowIfNullOrWhiteSpace(state);
            ArgumentException.ThrowIfNullOrWhiteSpace(zipCode);

            return new UserAddress
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Label = label,
                AddressLine1 = addressLine1,
                City = city,
                State = state,
                ZipCode = zipCode,
                Country = country,
                IsDefault = isDefault,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(
            string? label,
            string? addressLine1,
            string? city,
            string? state,
            string? zipCode,
            string? country)
        {
            if (!string.IsNullOrWhiteSpace(label)) Label = label;
            if (!string.IsNullOrWhiteSpace(addressLine1)) AddressLine1 = addressLine1;
            if (!string.IsNullOrWhiteSpace(city)) City = city;
            if (!string.IsNullOrWhiteSpace(state)) State = state;
            if (!string.IsNullOrWhiteSpace(zipCode)) ZipCode = zipCode;
            if (!string.IsNullOrWhiteSpace(country)) Country = country;
        }

        public void SetDefault(bool isDefault) => IsDefault = isDefault;
    }
}