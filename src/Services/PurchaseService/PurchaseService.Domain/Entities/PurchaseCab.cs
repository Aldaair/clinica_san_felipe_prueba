using PurchaseService.Domain.Enums;

namespace PurchaseService.Domain.Entities;

public sealed class PurchaseCab
{
    public int Id { get; private set; }
    public DateTime FecRegistro { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal Igv { get; private set; }
    public decimal Total { get; private set; }
    public PurchaseStatus Status { get; private set; }

    public List<PurchaseDet> Details { get; private set; } = new();

    private PurchaseCab() { }

    public PurchaseCab(DateTime fecRegistro, decimal subTotal, decimal igv, decimal total)
    {
        FecRegistro = fecRegistro;
        SubTotal = subTotal;
        Igv = igv;
        Total = total;
        Status = PurchaseStatus.Active;
    }

    public void AddDetail(int idProducto, decimal cantidad, decimal precio, decimal subTotal, decimal igv, decimal total)
    {
        Details.Add(new PurchaseDet(idProducto, cantidad, precio, subTotal, igv, total));
    }

    public void Cancel()
    {
        Status = PurchaseStatus.Cancelled;
    }
}