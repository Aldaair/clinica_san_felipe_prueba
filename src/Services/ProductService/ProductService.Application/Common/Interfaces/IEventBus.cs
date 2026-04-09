namespace ProductService.Application.Common.Interfaces;

public interface IEventBus
{
    Task PublishAsync(string eventType, string payload, CancellationToken cancellationToken = default);
}