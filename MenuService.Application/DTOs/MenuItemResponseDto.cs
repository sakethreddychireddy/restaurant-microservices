namespace MenuService.Application.DTOs
{
    public record MenuItemResponseDto(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        string Category,
        string Emoji,
        bool IsVegetarian,
        string? Badge,
        bool IsAvailable,
        DateTime CreatedAt,
        DateTime UpdatedAt
     );
}
