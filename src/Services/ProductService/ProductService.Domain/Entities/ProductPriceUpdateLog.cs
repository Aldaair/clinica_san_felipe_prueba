namespace ProductService.Domain.Entities;

public sealed class ProductPriceUpdateLog
{
    public int Id { get; private set; }
    public int PurchaseId { get; private set; }
    public int ProductId { get; private set; }

    public decimal OldCosto { get; private set; }
    public decimal OldPrecioVenta { get; private set; }

    public decimal NewCosto { get; private set; }
    public decimal NewPrecioVenta { get; private set; }

    public bool RolledBack { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? RolledBackAtUtc { get; private set; }

    private ProductPriceUpdateLog() { }

    public ProductPriceUpdateLog(
        int purchaseId,
        int productId,
        decimal oldCosto,
        decimal oldPrecioVenta,
        decimal newCosto,
        decimal newPrecioVenta)
    {
        PurchaseId = purchaseId;
        ProductId = productId;
        OldCosto = oldCosto;
        OldPrecioVenta = oldPrecioVenta;
        NewCosto = newCosto;
        NewPrecioVenta = newPrecioVenta;
        RolledBack = false;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void MarkRolledBack()
    {
        RolledBack = true;
        RolledBackAtUtc = DateTime.UtcNow;
    }
}