namespace OrderService.Application.Events
{
    public class OrderPlacedEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public List<OrderItemInfo> Items { get; set; } = new();
        public DateTime PlacedAt { get; set; }
        public record OrderItemInfo(Guid MenuItemId, string Name, decimal UnitPrice, int Quantity);
    }
}