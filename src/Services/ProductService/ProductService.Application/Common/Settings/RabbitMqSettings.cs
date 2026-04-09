namespace ProductService.Application.Common.Settings;

public sealed class RabbitMqSettings
{
    public string HostName { get; init; } = string.Empty;
    public int Port { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string VirtualHost { get; init; } = "/";
    public string ExchangeName { get; init; } = "product.events";
}