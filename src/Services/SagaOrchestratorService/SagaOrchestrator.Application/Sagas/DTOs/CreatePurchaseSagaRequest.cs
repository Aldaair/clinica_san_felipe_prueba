using System.ComponentModel.DataAnnotations;

namespace SagaOrchestrator.Application.Sagas.DTOs;

public sealed class CreatePurchaseSagaRequest
{
    [Required]
    [MinLength(1)]
    public List<PurchaseSagaItemRequest> Items { get; set; } = new();
}