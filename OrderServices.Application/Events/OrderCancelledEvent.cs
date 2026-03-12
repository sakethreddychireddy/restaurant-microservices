namespace OrderService.Application.Events
{
    public class OrderCancelledEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CancelledAt { get; set; }
    }
}