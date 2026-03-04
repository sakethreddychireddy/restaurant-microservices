using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs
{
    public record UpdateOrderStatusDto(OrderStatus Status);
}
