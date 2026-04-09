namespace ProductService.Application.Products.DTOs.Saga;

public sealed class ProductPurchaseBatchUpdateRequest
{
    public int PurchaseId { get; set; }
    public List<ProductPurchaseBatchUpdateItemRequest> Items { get; set; } = new();
}

public sealed class ProductPurchaseBatchUpdateItemRequest
{
    public int IdProducto { get; set; }
    public decimal NuevoCosto { get; set; }
}