namespace ProductService.Domain.Entities;

public sealed class Product
{
    public int Id { get; private set; }
    public string NombreProducto { get; private set; } = string.Empty;
    public string NroLote { get; private set; } = string.Empty;
    public DateTime FecRegistro { get; private set; }
    public decimal Costo { get; private set; }
    public decimal PrecioVenta { get; private set; }

    private Product() { }

    public Product(string nombreProducto, string nroLote, DateTime fecRegistro, decimal costo, decimal precioVenta)
    {
        NombreProducto = nombreProducto;
        NroLote = nroLote;
        FecRegistro = fecRegistro;
        Costo = costo;
        PrecioVenta = precioVenta;
    }

    public void Update(string nombreProducto, string nroLote, decimal costo, decimal precioVenta)
    {
        NombreProducto = nombreProducto;
        NroLote = nroLote;
        Costo = costo;
        PrecioVenta = precioVenta;
    }

    public void ApplyPurchaseUpdate(decimal nuevoCosto)
    {
        Costo = decimal.Round(nuevoCosto, 2, MidpointRounding.AwayFromZero);
        PrecioVenta = decimal.Round(nuevoCosto * 1.35m, 2, MidpointRounding.AwayFromZero);
    }

    public void RestorePrice(decimal costoAnterior, decimal precioVentaAnterior)
    {
        Costo = decimal.Round(costoAnterior, 2, MidpointRounding.AwayFromZero);
        PrecioVenta = decimal.Round(precioVentaAnterior, 2, MidpointRounding.AwayFromZero);
    }
}