using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SalesService.Application.Common.Settings;
using SalesService.Infrastructure.Persistence;
using SanFelipe.IntegrationEvents;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Microsoft.EntityFrameworkCore;

namespace SalesService.Infrastructure.Messaging;

public sealed class ProductPricingChangedConsumerBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqSettings _settings;
    private IConnection? _connection;
    private IChannel? _channel;

    public ProductPricingChangedConsumerBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqSettings> options)
    {
        _scopeFactory = scopeFactory;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(
            exchange: _settings.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            queue: "sales.product-pricing-snapshot",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await _channel.QueueBindAsync(
            queue: "sales.product-pricing-snapshot",
            exchange: _settings.ExchangeName,
            routingKey: "product.pricing.changed",
            cancellationToken: stoppingToken);

        await _channel.BasicQosAsync(0, 10, false, stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SalesDbContext>();

            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var evt = JsonSerializer.Deserialize<ProductPricingChangedIntegrationEvent>(json);

                if (evt is null)
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false, stoppingToken);
                    return;
                }

                var alreadyProcessed = await db.ProcessedIntegrationEvents
                    .AnyAsync(x => x.EventId == evt.EventId, stoppingToken);

                if (!alreadyProcessed)
                {
                    var snapshot = await db.ProductPricingSnapshots
                        .FirstOrDefaultAsync(x => x.IdProducto == evt.IdProducto, stoppingToken);

                    if (snapshot is null)
                    {
                        snapshot = new SalesService.Domain.Entities.ProductPricingSnapshot(
                            evt.IdProducto,
                            evt.NombreProducto,
                            evt.PrecioVenta);

                        await db.ProductPricingSnapshots.AddAsync(snapshot, stoppingToken);
                    }
                    else
                    {
                        snapshot.Update(evt.NombreProducto, evt.PrecioVenta);
                    }

                    db.ProcessedIntegrationEvents.Add(new SalesService.Domain.Entities.ProcessedIntegrationEvent
                    {
                        EventId = evt.EventId,
                        EventType = nameof(ProductPricingChangedIntegrationEvent),
                        ProcessedAtUtc = DateTime.UtcNow
                    });

                    await db.SaveChangesAsync(stoppingToken);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "sales.product-pricing-snapshot",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null) await _channel.DisposeAsync();
        _connection?.Dispose();
        await base.StopAsync(cancellationToken);
    }
}