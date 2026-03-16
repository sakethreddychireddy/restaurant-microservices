namespace MenuService.Application.DTOs
{
    public record UpdateMenuItemDto(
        string? Name,
        string? Description,
        decimal? Price,
        string? Category,
        string? ImageUrl,
        bool? IsVegetarian,
        string? Badge,
        bool? IsAvailable
    );
    public record UpdateImageUrlDto(string ImageUrl);
}
