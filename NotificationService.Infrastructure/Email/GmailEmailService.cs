using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using NotificationService.Application.Interfaces;

namespace NotificationService.Infrastructure.Email
{
    public class GmailEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GmailEmailService> _logger;

        public GmailEmailService(
            IConfiguration configuration,
            ILogger<GmailEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendAsync(
            string toEmail,
            string toName,
            string subject,
            string htmlBody,
            CancellationToken ct = default)
        {
            var fromEmail = _configuration["Email:FromEmail"]!;
            var fromName = _configuration["Email:FromName"]!;
            var password = _configuration["Email:AppPassword"]!;
            var host = _configuration["Email:Host"] ?? "smtp.gmail.com";
            var port = int.Parse(_configuration["Email:Port"] ?? "587");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            message.Body = new BodyBuilder
            {
                HtmlBody = htmlBody
            }.ToMessageBody();

            using var client = new SmtpClient();

            _logger.LogInformation(
                "Connecting to SMTP {Host}:{Port}", host, port);

            await client.ConnectAsync(
                host, port, SecureSocketOptions.StartTls, ct);

            await client.AuthenticateAsync(fromEmail, password, ct);

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            _logger.LogInformation(
                "Email sent to {ToEmail} — Subject: {Subject}",
                toEmail, subject);
        }
    }
}