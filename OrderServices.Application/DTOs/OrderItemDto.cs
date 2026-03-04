namespace OrderService.Application.DTOs
{
    public record OrderItemDto(
        Guid MenuItemId,
        int Quantity);
}
