namespace SalesService.Application.Sales.DTOs;

public sealed record SaleResponse(
    int IdVentaCab,
    DateTime FecRegistro,
    decimal SubTotal,
    decimal Igv,
    decimal Total,
    string Status,
    IReadOnlyList<SaleDetailResponse> Details
);