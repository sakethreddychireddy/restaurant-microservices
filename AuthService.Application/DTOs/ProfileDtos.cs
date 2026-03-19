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
        string FullAddress,
        bool IsDefault = false);

    public record AddressResponse(
        Guid Id,
        string Label,
        string FullAddress,
        bool IsDefault,
        DateTime CreatedAt);
}