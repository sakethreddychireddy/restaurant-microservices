namespace AuthService.Application.DTOs
{
    public record UpdateProfileRequest(
        string? Name,
        string? Phone);

    public record ProfileResponse(
        Guid UserId,
        string Name,
        string Email,
        string? Phone,
        string Role,
        bool IsOAuthUser);

    public record AddressRequest(
        string Label,
        string AddressLine1,
        string City,
        string State,
        string ZipCode,
        string Country,
        bool IsDefault = false);

    public record AddressResponse(
        Guid Id,
        string Label,
        string AddressLine1,
        string City,
        string State,
        string ZipCode,
        string Country,
        bool IsDefault,
        DateTime CreatedAt)
    {
        public string FullAddress =>
            $"{AddressLine1}, {City}, {State} {ZipCode}, {Country}";

        public string ShortAddress =>
            $"{AddressLine1}, {City}";
    }
}