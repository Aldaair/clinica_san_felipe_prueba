namespace MovementService.Application.Movements.DTOs;

public sealed record StockResponse(
    int IdProducto,
    decimal StockActual
);