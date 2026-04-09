using System.ComponentModel.DataAnnotations;

namespace SalesService.Application.Sales.DTOs;

public sealed class ProductPricingSnapshotUpsertRequest
{
    [Range(1, int.MaxValue)]
    public int IdProducto { get; set; }

    [Required]
    [MaxLength(200)]
    public string NombreProducto { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal PrecioVenta { get; set; }
}