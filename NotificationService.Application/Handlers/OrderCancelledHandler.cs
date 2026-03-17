using Microsoft.Extensions.Logging;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Templates;
using NotificationService.Domain.Entities;
using Shared.Contract.Events;

namespace NotificationService.Application.Handlers
{
    public class OrderCancelledHandler
    {
        private readonly IEmailService _emailService;
        private readonly INotificationLogRepository _logRepository;
        private readonly ILogger<OrderCancelledHandler> _logger;

        public OrderCancelledHandler(
            IEmailService emailService,
            INotificationLogRepository logRepository,
            ILogger<OrderCancelledHandler> logger)
        {
            _emailService = emailService;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task HandleAsync(
            OrderCancelledEvent evt, CancellationToken ct = default)
        {
            _logger.LogInformation(
                "Sending order cancelled email to {Email} for order {OrderId}",
                evt.UserEmail, evt.OrderId);

            var subject =
                $"❌ Order Cancelled — #{evt.OrderId.ToString().ToUpper()[..8]}";

            var html = EmailTemplates.OrderCancelled(
                evt.UserName, evt.OrderId);

            NotificationLog log;
            try
            {
                await _emailService.SendAsync(
                    evt.UserEmail, evt.UserName, subject, html, ct);

                log = NotificationLog.Create(
                    evt.UserEmail, subject, "OrderCancelled",
                    evt.OrderId, isSuccess: true);

                _logger.LogInformation(
                    "Order cancelled email sent to {Email}", evt.UserEmail);
            }
            catch (Exception ex)
            {
                log = NotificationLog.Create(
                    evt.UserEmail, subject, "OrderCancelled",
                    evt.OrderId, isSuccess: false, ex.Message);

                _logger.LogError(ex,
                    "Failed to send order cancelled email to {Email}", evt.UserEmail);
            }

            await _logRepository.AddAsync(log, ct);
            await _logRepository.SaveChangesAsync(ct);
        }
    }
}