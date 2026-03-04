namespace OrderService.Application.DTOs
{
    public record OrderItemResponseDto(
        Guid MenuItemId,
        string MenuItemName,
        decimal UnitPrice,
        int Quantity,
        decimal Subtotal
    );
}
