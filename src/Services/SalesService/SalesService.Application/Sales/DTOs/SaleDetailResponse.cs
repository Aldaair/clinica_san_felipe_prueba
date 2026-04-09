namespace SalesService.Application.Sales.DTOs;

public sealed record SaleDetailResponse(
    int IdProducto,
    decimal Cantidad,
    decimal Precio,
    decimal SubTotal,
    decimal Igv,
    decimal Total
);