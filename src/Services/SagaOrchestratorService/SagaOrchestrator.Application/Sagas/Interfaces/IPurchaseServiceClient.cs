using SagaOrchestrator.Application.Sagas.DTOs;

namespace SagaOrchestrator.Application.Sagas.Interfaces;

public interface IPurchaseServiceClient
{
    Task<PurchaseCreateResponse> CreatePurchaseAsync(CreatePurchaseSagaRequest request, CancellationToken cancellationToken = default);
    Task CancelPurchaseAsync(int purchaseId, CancellationToken cancellationToken = default);
}