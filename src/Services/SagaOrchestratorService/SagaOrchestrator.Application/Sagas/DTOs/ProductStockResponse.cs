namespace SagaOrchestrator.Application.Sagas.DTOs;

public sealed record ProductStockResponse(
    int IdProducto,
    decimal StockActual
);