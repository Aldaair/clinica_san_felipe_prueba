namespace ProductService.Application.Products.DTOs.Saga;

public sealed class ProductPurchaseBatchRollbackRequest
{
    public int PurchaseId { get; set; }
}