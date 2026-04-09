using System.ComponentModel.DataAnnotations;

namespace SalesService.Application.Sales.DTOs;

public sealed class CreateSaleItemRequest
{
    [Range(1, int.MaxValue)]
    public int IdProducto { get; set; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Cantidad { get; set; }
}