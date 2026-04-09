namespace ProductService.Application.Products.DTOs;

public sealed record ProductResponse(
    int Id,
    string NombreProducto,
    string NroLote,
    DateTime FecRegistro,
    decimal Costo,
    decimal PrecioVenta
);