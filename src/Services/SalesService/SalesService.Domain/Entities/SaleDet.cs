namespace SalesService.Domain.Entities;

public sealed class SaleDet
{
    public int Id { get; private set; }
    public int IdVentaCab { get; private set; }
    public int IdProducto { get; private set; }
    public decimal Cantidad { get; private set; }
    public decimal Precio { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal Igv { get; private set; }
    public decimal Total { get; private set; }

    private SaleDet() { }

    internal SaleDet(int idProducto, decimal cantidad, decimal precio, decimal subTotal, decimal igv, decimal total)
    {
        IdProducto = idProducto;
        Cantidad = cantidad;
        Precio = precio;
        SubTotal = subTotal;
        Igv = igv;
        Total = total;
    }
}