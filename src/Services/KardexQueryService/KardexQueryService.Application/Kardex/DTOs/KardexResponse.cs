namespace KardexQueryService.Application.Kardex.DTOs;

public sealed record KardexResponse(
    int IdProducto,
    string NombreProducto,
    decimal StockActual,
    decimal Costo,
    decimal PrecioVenta
);