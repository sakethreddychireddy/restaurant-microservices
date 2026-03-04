namespace OrderService.Application.DTOs
{
    public record CreateOrderDto(
        string DeliveryAddress,
        List<OrderItemDto> Items);
}
