using System.ComponentModel.DataAnnotations;

namespace SagaOrchestrator.Application.Sagas.DTOs;

public sealed class CreateSaleSagaRequest
{
    [Required]
    [MinLength(1)]
    public List<SaleSagaItemRequest> Items { get; set; } = new();
}