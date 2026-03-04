namespace MenuService.Application.DTOs
{
    public record UpdateMenuItemDto(
        string? Name,
        string? Description,
        decimal? Price,
        string? Category,
        string? Emoji,
        bool? IsVegetarian,
        string? Badge,
        bool? IsAvailable
    );
}
