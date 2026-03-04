namespace OrderService.Infrastructure.Messaging.Events
{
    public class OrderStatusChangedEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string NewStatus { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
    }
}
