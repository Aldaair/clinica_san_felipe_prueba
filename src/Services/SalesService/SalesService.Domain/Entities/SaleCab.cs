using SalesService.Domain.Enums;

namespace SalesService.Domain.Entities;

public sealed class SaleCab
{
    public int Id { get; private set; }
    public DateTime FecRegistro { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal Igv { get; private set; }
    public decimal Total { get; private set; }
    public SaleStatus Status { get; private set; }

    public List<SaleDet> Details { get; private set; } = new();

    private SaleCab() { }

    public SaleCab(DateTime fecRegistro, decimal subTotal, decimal igv, decimal total)
    {
        FecRegistro = fecRegistro;
        SubTotal = subTotal;
        Igv = igv;
        Total = total;
        Status = SaleStatus.Active;
    }

    public void AddDetail(int idProducto, decimal cantidad, decimal precio, decimal subTotal, decimal igv, decimal total)
    {
        Details.Add(new SaleDet(idProducto, cantidad, precio, subTotal, igv, total));
    }

    public void Cancel()
    {
        Status = SaleStatus.Cancelled;
    }
}