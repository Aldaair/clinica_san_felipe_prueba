using System.ComponentModel.DataAnnotations;

namespace PurchaseService.Application.Purchases.DTOs;

public sealed class CreatePurchaseItemRequest
{
    [Range(1, int.MaxValue)]
    public int IdProducto { get; set; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Cantidad { get; set; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Precio { get; set; }
}