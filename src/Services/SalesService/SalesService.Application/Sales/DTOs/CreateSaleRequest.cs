using System.ComponentModel.DataAnnotations;

namespace SalesService.Application.Sales.DTOs;

public sealed class CreateSaleRequest
{
    [Required]
    [MinLength(1)]
    public List<CreateSaleItemRequest> Items { get; set; } = new();
}