using System.Text;
using Microsoft.Extensions.Options;
using ProductService.Application.Common.Interfaces;
using ProductService.Application.Common.Settings;
using RabbitMQ.Client;

namespace ProductService.Infrastructure.Messaging;

public sealed class RabbitMqEventBus : IEventBus, IAsyncDisposable
{
    private readonly RabbitMqSettings _settings;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqEventBus(IOptions<RabbitMqSettings> options)
    {
        _settings = options.Value;
    }

    private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        if (_connection is not null && _channel is not null)
            return;

        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange: _settings.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);
    }

    public async Task PublishAsync(string eventType, string payload, CancellationToken cancellationToken = default)
    {
        await EnsureConnectedAsync(cancellationToken);

        var body = Encoding.UTF8.GetBytes(payload);

        var props = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            Type = eventType
        };

        await _channel!.BasicPublishAsync(
            exchange: _settings.ExchangeName,
            routingKey: "product.pricing.changed",
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null) await _channel.DisposeAsync();
        _connection?.Dispose();
    }
}