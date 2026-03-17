using Microsoft.Extensions.Logging;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Templates;
using NotificationService.Domain.Entities;
using Shared.Contract.Events;

namespace NotificationService.Application.Handlers
{
    public class OrderStatusChangedHandler
    {
        private readonly IEmailService _emailService;
        private readonly INotificationLogRepository _logRepository;
        private readonly ILogger<OrderStatusChangedHandler> _logger;

        public OrderStatusChangedHandler(
            IEmailService emailService,
            INotificationLogRepository logRepository,
            ILogger<OrderStatusChangedHandler> logger)
        {
            _emailService = emailService;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task HandleAsync(
            OrderStatusChangedEvent evt, CancellationToken ct = default)
        {
            _logger.LogInformation(
                "Sending status changed email to {Email} for order {OrderId} — {Status}",
                evt.UserEmail, evt.OrderId, evt.NewStatus);

            var subject =
                $"📋 Order Update — {evt.NewStatus} — #{evt.OrderId.ToString().ToUpper()[..8]}";

            var html = EmailTemplates.OrderStatusChanged(
                evt.UserName, evt.OrderId, evt.NewStatus);

            NotificationLog log;
            try
            {
                await _emailService.SendAsync(
                    evt.UserEmail, evt.UserName, subject, html, ct);

                log = NotificationLog.Create(
                    evt.UserEmail, subject, "OrderStatusChanged",
                    evt.OrderId, isSuccess: true);

                _logger.LogInformation(
                    "Status changed email sent to {Email}", evt.UserEmail);
            }
            catch (Exception ex)
            {
                log = NotificationLog.Create(
                    evt.UserEmail, subject, "OrderStatusChanged",
                    evt.OrderId, isSuccess: false, ex.Message);

                _logger.LogError(ex,
                    "Failed to send status changed email to {Email}", evt.UserEmail);
            }

            await _logRepository.AddAsync(log, ct);
            await _logRepository.SaveChangesAsync(ct);
        }
    }
}