using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string UserEmail { get; private set; } = string.Empty;
        public string UserName { get; private set; } = string.Empty;
        public string DeliveryAddress { get; private set; } = string.Empty;
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public decimal TotalPrice { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private Order() { }

        public static Order Create(
            Guid userId,
            string userEmail,
            string userName,
            string deliveryAddress,
            List<OrderItem> items)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(deliveryAddress);
            if (items == null || items.Count == 0)
                throw new ArgumentException("Order must have at least one item.", nameof(items));

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UserEmail = userEmail,
                UserName = userName,
                DeliveryAddress = deliveryAddress,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            order._items.AddRange(items);
            order.TotalPrice = items.Sum(i => i.Subtotal);
            return order;
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel a delivered order.");
            Status = OrderStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
