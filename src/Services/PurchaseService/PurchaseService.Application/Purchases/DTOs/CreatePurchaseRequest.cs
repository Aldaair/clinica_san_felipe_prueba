using System.ComponentModel.DataAnnotations;

namespace PurchaseService.Application.Purchases.DTOs;

public sealed class CreatePurchaseRequest
{
    [Required]
    [MinLength(1)]
    public List<CreatePurchaseItemRequest> Items { get; set; } = new();
}