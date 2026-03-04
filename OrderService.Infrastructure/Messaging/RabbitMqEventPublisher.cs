using Microsoft.Extensions.Configuration;
using OrderService.Application.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderService.Infrastructure.Messaging;

//public class RabbitMqPublisher : IEventPublisher, IDisposable
//{
//    private readonly IConnection _connection;
//    private readonly RabbitMQ.Client.IModel _channel;
//    private const string ExchangeName = "order.events";

//    public RabbitMqPublisher(IConfiguration configuration)
//    {
//        var factory = new ConnectionFactory
//        {
//            HostName = configuration["RabbitMq:Host"] ?? "localhost",
//            Port = int.Parse(configuration["RabbitMq:Port"] ?? "5672"),
//            UserName = configuration["RabbitMq:Username"] ?? "guest",
//            Password = configuration["RabbitMq:Password"] ?? "guest"
//        };

//        _connection = factory.CreateConnection();
//        _channel = _connection.CreateModel();

//        _channel.ExchangeDeclare(
//            exchange: ExchangeName,
//            type: ExchangeType.Topic,
//            durable: true);
//    }

//    public Task PublishAsync<T>(T message, CancellationToken ct = default)
//        where T : class
//    {
//        ct.ThrowIfCancellationRequested();

//        var routingKey = typeof(T).Name.ToLowerInvariant();
//        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

//        var properties = _channel.CreateBasicProperties();
//        properties.Persistent = true;
//        properties.ContentType = "application/json";
//        properties.Headers = new Dictionary<string, object>
//        {
//            { "event-type", typeof(T).Name }
//        };

//        _channel.BasicPublish(
//            exchange: ExchangeName,
//            routingKey: routingKey,
//            basicProperties: properties,
//            body: body);

//        return Task.CompletedTask;
//    }

//    public void Dispose()
//    {
//        _channel?.Close();
//        _connection?.CloseAsync();
//    }
//}
public class RabbitMqPublisher : IEventPublisher, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly ConnectionFactory _factory;
    private const string ExchangeName = "order.events";

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:Host"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMq:Port"] ?? "5672"),
            UserName = configuration["RabbitMq:Username"] ?? "guest",
            Password = configuration["RabbitMq:Password"] ?? "guest"
        };
    }

    private async Task InitializeAsync(CancellationToken ct)
    {
        if (_connection == null)
        {
            // v7 uses CreateConnectionAsync
            _connection = await _factory.CreateConnectionAsync(ct);
            // v7 uses CreateChannelAsync (replacing CreateModel)
            _channel = await _connection.CreateChannelAsync(cancellationToken: ct);

            // v7 ExchangeDeclareAsync
            await _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                cancellationToken: ct);
        }
    }

    public async Task PublishAsync<T>(T message, CancellationToken ct = default)
        where T : class
    {
        await InitializeAsync(ct);

        var routingKey = typeof(T).Name.ToLowerInvariant();
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        // In v7, properties are handled via BasicProperties class directly
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            Headers = new Dictionary<string, object?>
            {
                { "event-type", typeof(T).Name }
            }
        };

        // v7 uses BasicPublishAsync
        await _channel!.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null) await _channel.CloseAsync();
        if (_connection != null) await _connection.CloseAsync();
    }
}