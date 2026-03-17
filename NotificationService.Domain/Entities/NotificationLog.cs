namespace NotificationService.Domain.Entities
{
    public class NotificationLog
    {
        public Guid Id { get; private set; }
        public string RecipientEmail { get; private set; } = string.Empty;
        public string Subject { get; private set; } = string.Empty;
        public string EventType { get; private set; } = string.Empty;
        public Guid OrderId { get; private set; }
        public bool IsSuccess { get; private set; }
        public string? ErrorMessage { get; private set; }
        public DateTime SentAt { get; private set; }

        private NotificationLog() { }

        public static NotificationLog Create(
            string recipientEmail,
            string subject,
            string eventType,
            Guid orderId,
            bool isSuccess,
            string? errorMessage = null)
        {
            return new NotificationLog
            {
                Id = Guid.NewGuid(),
                RecipientEmail = recipientEmail,
                Subject = subject,
                EventType = eventType,
                OrderId = orderId,
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage,
                SentAt = DateTime.UtcNow
            };
        }
    }
}