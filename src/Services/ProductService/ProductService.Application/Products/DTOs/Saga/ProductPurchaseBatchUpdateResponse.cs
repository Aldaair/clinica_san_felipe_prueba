namespace ProductService.Application.Products.DTOs.Saga;

public sealed record ProductPurchaseBatchUpdateResponse(
    int PurchaseId,
    int UpdatedProductsCount,
    string Message
);