using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs
{
    public record OrderResponseDto(
        Guid Id,
        Guid UserId,
        string DeliveryAddress,
        OrderStatus Status,
        decimal TotalPrice,
        List<OrderItemResponseDto> Items,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
