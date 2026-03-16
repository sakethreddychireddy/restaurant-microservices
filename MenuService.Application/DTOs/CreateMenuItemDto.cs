namespace MenuService.Application.DTOs
{
    public record CreateMenuItemDto(
        string Name,
        string Description,
        decimal Price,
        string Category,
        string ImageUrl,
        bool IsVegetarian,
        string? Badge = null
    );
}
