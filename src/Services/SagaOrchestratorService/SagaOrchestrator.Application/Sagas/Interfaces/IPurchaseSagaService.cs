using SagaOrchestrator.Application.Sagas.DTOs;

namespace SagaOrchestrator.Application.Sagas.Interfaces;

public interface IPurchaseSagaService
{
    Task<PurchaseSagaResponse> CreateAsync(CreatePurchaseSagaRequest request, CancellationToken cancellationToken = default);
    Task<PurchaseSagaResponse?> GetByIdAsync(int sagaId, CancellationToken cancellationToken = default);
}