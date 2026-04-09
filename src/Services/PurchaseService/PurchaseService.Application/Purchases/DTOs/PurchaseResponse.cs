namespace PurchaseService.Application.Purchases.DTOs;

public sealed record PurchaseResponse(
    int IdCompraCab,
    DateTime FecRegistro,
    decimal SubTotal,
    decimal Igv,
    decimal Total,
    string Status,
    IReadOnlyList<PurchaseDetailResponse> Details
);