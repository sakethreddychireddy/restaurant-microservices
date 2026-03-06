using FluentValidation;
using OrderService.Domain.Entities;
using OrderService.Domain.Exceptions;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Messaging.Events;

namespace OrderService.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IMenuServiceClient _menuClient;
        private readonly IEventPublisher _publisher;
        private readonly IValidator<CreateOrderDto> _validator;

        public OrderService(IOrderRepository repository, IMenuServiceClient menuClient,
            IEventPublisher publisher, IValidator<CreateOrderDto> validator)
        {
            _repository = repository;
            _menuClient = menuClient;
            _publisher = publisher;
            _validator = validator;
        }

        public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto,
            Guid userId, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(dto, ct);

            // Validate menu items via gRPC
            var menuItemIds = dto.Items.Select(i => i.MenuItemId);
            var menuItems = (await _menuClient.GetMenuItemsAsync(menuItemIds, ct)).ToList();

            // Check all items exist and are available
            foreach (var requested in dto.Items)
            {
                var menuItem = menuItems.FirstOrDefault(m => m.Id == requested.MenuItemId)
                    ?? throw new InvalidOperationException(
                        $"Menu item '{requested.MenuItemId}' not found.");

                if (!menuItem.IsAvailable)
                    throw new InvalidOperationException(
                        $"Menu item '{menuItem.Name}' is currently unavailable.");
            }

            // Build order items using real prices from Menu Service
            var orderId = Guid.NewGuid();
            var orderItems = dto.Items.Select(i =>
            {
                var menuItem = menuItems.First(m => m.Id == i.MenuItemId);
                return OrderItem.Create(orderId, i.MenuItemId, menuItem.Name, menuItem.Price, i.Quantity);
            }).ToList();

            var order = Order.Create(userId, dto.DeliveryAddress, orderItems);

            await _repository.AddAsync(order, ct);

            // Publish OrderPlaced event to RabbitMQ
            await _publisher.PublishAsync(new OrderPlacedEvent
            {
                OrderId = order.Id,
                UserId = order.UserId,
                TotalPrice = order.TotalPrice,
                DeliveryAddress = order.DeliveryAddress,
                Items = orderItems.Select(i => new OrderPlacedEvent.OrderItemInfo(
                    i.MenuItemId, i.MenuItemName, i.UnitPrice, i.Quantity)).ToList(),
                PlacedAt = order.CreatedAt
            }, ct);

            return ToResponse(order);
        }

        public async Task<OrderResponseDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var order = await _repository.GetByIdAsync(id, ct)
                ?? throw new OrderNotFoundException(id);
            return ToResponse(order);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(
            Guid userId, CancellationToken ct = default)
        {
            var orders = await _repository.GetByUserIdAsync(userId, ct);
            return orders.Select(ToResponse);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllAsync(CancellationToken ct = default)
        {
            var orders = await _repository.GetAllAsync(ct);
            return orders.Select(ToResponse);
        }

        public async Task<OrderResponseDto> UpdateStatusAsync(Guid id,
            UpdateOrderStatusDto dto, CancellationToken ct = default)
        {
            var order = await _repository.GetByIdAsync(id, ct)
                ?? throw new OrderNotFoundException(id);

            order.UpdateStatus(dto.status);
            await _repository.UpdateAsync(order, ct);

            // Publish status changed event
            await _publisher.PublishAsync(new OrderStatusChangedEvent
            {
                OrderId = order.Id,
                UserId = order.UserId,
                NewStatus = dto.status.ToString(),
                ChangedAt = order.UpdatedAt
            }, ct);

            return ToResponse(order);
        }

        public async Task CancelAsync(Guid id, CancellationToken ct = default)
        {
            var order = await _repository.GetByIdAsync(id, ct)
                ?? throw new OrderNotFoundException(id);

            order.Cancel();
            await _repository.UpdateAsync(order, ct);

            await _publisher.PublishAsync(new OrderCancelledEvent
            {
                OrderId = order.Id,
                UserId = order.UserId,
                CancelledAt = order.UpdatedAt
            }, ct);
        }

        private static OrderResponseDto ToResponse(Order order) => new(
            order.Id,
            order.UserId,
            order.DeliveryAddress,
            order.Status,
            order.TotalPrice,
            order.Items.Select(i => new OrderItemResponseDto(
                i.MenuItemId, i.MenuItemName, i.UnitPrice, i.Quantity, i.Subtotal)).ToList(),
            order.CreatedAt,
            order.UpdatedAt
        );
    }
}
