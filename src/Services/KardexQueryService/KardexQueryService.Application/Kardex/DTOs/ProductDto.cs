namespace KardexQueryService.Application.Kardex.DTOs;

public sealed record ProductDto(
    int Id,
    string NombreProducto,
    decimal Costo,
    decimal PrecioVenta
);