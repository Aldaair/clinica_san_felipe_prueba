namespace SanFelipe.IntegrationEvents;

public sealed record ProductPricingChangedIntegrationEvent(
    Guid EventId,
    DateTime OccurredAtUtc,
    int IdProducto,
    string NombreProducto,
    decimal PrecioVenta
);