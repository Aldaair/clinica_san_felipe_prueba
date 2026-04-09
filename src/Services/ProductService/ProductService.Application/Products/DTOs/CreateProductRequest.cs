using System.ComponentModel.DataAnnotations;

namespace ProductService.Application.Products.DTOs;

public sealed class CreateProductRequest
{
    [Required]
    [MaxLength(200)]
    public string NombreProducto { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string NroLote { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Costo { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal PrecioVenta { get; set; }
}