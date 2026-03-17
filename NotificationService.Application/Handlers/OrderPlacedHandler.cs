using NotificationService.Application.Interfaces;
using NotificationService.Application.Templates;
using NotificationService.Domain.Entities;
using Shared.Contract.Events;
using Microsoft.Extensions.Logging;

namespace NotificationService.Application.Handlers
{
    public class OrderPlacedHandler
    {
        private readonly IEmailService _emailService;
        private readonly INotificationLogRepository _logRepository;
        private readonly ILogger<OrderPlacedHandler> _logger;

        public OrderPlacedHandler(
            IEmailService emailService,
            INotificationLogRepository logRepository,
            ILogger<OrderPlacedHandler> logger)
        {
            _emailService = emailService;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task HandleAsync(
            OrderPlacedEvent evt, CancellationToken ct = default)
        {
            _logger.LogInformation(
                "Sending order placed email to {Email} for order {OrderId}",
                evt.UserEmail, evt.OrderId);

            var subject = $"🎉 Order Confirmed — #{evt.OrderId.ToString().ToUpper()[..8]}";

            var items = evt.Items
                .Select(i => (i.Name, i.UnitPrice, i.Quantity))
                .ToList();

            var html = EmailTemplates.OrderPlaced(
                evt.UserName,
                evt.OrderId,
                evt.TotalPrice,
                evt.DeliveryAddress,
                items);

            NotificationLog log;
            try
            {
                await _emailService.SendAsync(
                    evt.UserEmail, evt.UserName, subject, html, ct);

                log = NotificationLog.Create(
                    evt.UserEmail, subject, "OrderPlaced",
                    evt.OrderId, isSuccess: true);

                _logger.LogInformation(
                    "Order placed email sent successfully to {Email}", evt.UserEmail);
            }
            catch (Exception ex)
            {
                log = NotificationLog.Create(
                    evt.UserEmail, subject, "OrderPlaced",
                    evt.OrderId, isSuccess: false, ex.Message);

                _logger.LogError(ex,
                    "Failed to send order placed email to {Email}", evt.UserEmail);
            }

            await _logRepository.AddAsync(log, ct);
            await _logRepository.SaveChangesAsync(ct);
        }
    }
}