using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Handlers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contract.Events;

namespace NotificationService.Infrastructure.Messaging
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RabbitMqConsumer> _logger;

        private IConnection? _connection;
        private IChannel? _channel;

        private const string ExchangeName = "order.events";
        private const string QueueName = "notification.queue";

        public RabbitMqConsumer(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            ILogger<RabbitMqConsumer> logger)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("RabbitMQ Consumer starting...");

            await InitializeAsync(ct);
            await StartConsumingAsync(ct);

            // keep alive until cancelled
            await Task.Delay(Timeout.Infinite, ct);
        }

        private async Task InitializeAsync(CancellationToken ct)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMq:Host"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMq:Port"] ?? "5672"),
                UserName = _configuration["RabbitMq:Username"] ?? "guest",
                Password = _configuration["RabbitMq:Password"] ?? "guest"
            };

            // retry connection up to 5 times
            for (int attempt = 1; attempt <= 5; attempt++)
            {
                try
                {
                    _logger.LogInformation(
                        "Connecting to RabbitMQ attempt {Attempt}/5...", attempt);

                    _connection = await factory.CreateConnectionAsync(ct);
                    _channel = await _connection.CreateChannelAsync(
                        cancellationToken: ct);

                    // declare exchange
                    await _channel.ExchangeDeclareAsync(
                        exchange: ExchangeName,
                        type: ExchangeType.Topic,
                        durable: true,
                        cancellationToken: ct);

                    // declare queue
                    await _channel.QueueDeclareAsync(
                        queue: QueueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        cancellationToken: ct);

                    // bind queue to exchange for all order events
                    await _channel.QueueBindAsync(
                        queue: QueueName,
                        exchange: ExchangeName,
                        routingKey: "orderplacedevent",
                        cancellationToken: ct);

                    await _channel.QueueBindAsync(
                        queue: QueueName,
                        exchange: ExchangeName,
                        routingKey: "orderstatuschangedevent",
                        cancellationToken: ct);

                    await _channel.QueueBindAsync(
                        queue: QueueName,
                        exchange: ExchangeName,
                        routingKey: "ordercancelledevent",
                        cancellationToken: ct);

                    _logger.LogInformation(
                        "RabbitMQ connected and queue bound successfully");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        "RabbitMQ connection attempt {Attempt} failed: {Message}",
                        attempt, ex.Message);

                    if (attempt == 5) throw;
                    await Task.Delay(3000 * attempt, ct);
                }
            }
        }

        private async Task StartConsumingAsync(CancellationToken ct)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel!);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                _logger.LogInformation(
                    "Received message with routing key: {RoutingKey}", routingKey);

                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    switch (routingKey.ToLower())
                    {
                        case "orderplacedevent":
                            var placedEvt = JsonSerializer.Deserialize<OrderPlacedEvent>(
                                message, JsonOptions);
                            if (placedEvt is not null)
                            {
                                var handler = scope.ServiceProvider
                                    .GetRequiredService<OrderPlacedHandler>();
                                await handler.HandleAsync(placedEvt, ct);
                            }
                            break;

                        case "orderstatuschangedevent":
                            var statusEvt = JsonSerializer.Deserialize<OrderStatusChangedEvent>(
                                message, JsonOptions);
                            if (statusEvt is not null)
                            {
                                var handler = scope.ServiceProvider
                                    .GetRequiredService<OrderStatusChangedHandler>();
                                await handler.HandleAsync(statusEvt, ct);
                            }
                            break;

                        case "ordercancelledevent":
                            var cancelledEvt = JsonSerializer.Deserialize<OrderCancelledEvent>(
                                message, JsonOptions);
                            if (cancelledEvt is not null)
                            {
                                var handler = scope.ServiceProvider
                                    .GetRequiredService<OrderCancelledHandler>();
                                await handler.HandleAsync(cancelledEvt, ct);
                            }
                            break;

                        default:
                            _logger.LogWarning(
                                "Unknown routing key: {RoutingKey}", routingKey);
                            break;
                    }

                    // acknowledge message
                    await _channel!.BasicAckAsync(
                        ea.DeliveryTag, multiple: false, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error processing message with routing key {RoutingKey}",
                        routingKey);

                    // reject and requeue once
                    await _channel!.BasicNackAsync(
                        ea.DeliveryTag, multiple: false,
                        requeue: false, ct);
                }
            };

            await _channel!.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: ct);

            _logger.LogInformation(
                "Started consuming from queue: {QueueName}", QueueName);
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public override async Task StopAsync(CancellationToken ct)
        {
            _logger.LogInformation("RabbitMQ Consumer stopping...");
            if (_channel is not null) await _channel.CloseAsync(ct);
            if (_connection is not null) await _connection.CloseAsync(ct);
            await base.StopAsync(ct);
        }
    }
}