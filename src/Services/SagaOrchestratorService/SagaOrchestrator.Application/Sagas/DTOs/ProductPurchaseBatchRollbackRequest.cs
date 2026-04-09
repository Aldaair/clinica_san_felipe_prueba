namespace SagaOrchestrator.Application.Sagas.DTOs;

public sealed class ProductPurchaseBatchRollbackRequest
{
    public int PurchaseId { get; set; }
}