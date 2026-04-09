namespace SalesService.Domain.Entities;

public sealed class ProductPricingSnapshot
{
    public int IdProducto { get; private set; }
    public string NombreProducto { get; private set; } = string.Empty;
    public decimal PrecioVenta { get; private set; }
    public DateTime LastUpdatedUtc { get; private set; }

    private ProductPricingSnapshot() { }

    public ProductPricingSnapshot(int idProducto, string nombreProducto, decimal precioVenta)
    {
        IdProducto = idProducto;
        NombreProducto = nombreProducto;
        PrecioVenta = precioVenta;
        LastUpdatedUtc = DateTime.UtcNow;
    }

    public void Update(string nombreProducto, decimal precioVenta)
    {
        NombreProducto = nombreProducto;
        PrecioVenta = precioVenta;
        LastUpdatedUtc = DateTime.UtcNow;
    }
}