namespace KardexQueryService.Application.Kardex.DTOs;

public sealed record StockDto(
    int IdProducto,
    decimal StockActual
);