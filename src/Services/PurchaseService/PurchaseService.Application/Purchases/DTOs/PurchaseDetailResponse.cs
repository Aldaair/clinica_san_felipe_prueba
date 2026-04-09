namespace PurchaseService.Application.Purchases.DTOs;

public sealed record PurchaseDetailResponse(
    int IdProducto,
    decimal Cantidad,
    decimal Precio,
    decimal SubTotal,
    decimal Igv,
    decimal Total
);