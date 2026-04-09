using SagaOrchestrator.Application.Sagas.DTOs;

namespace SagaOrchestrator.Application.Sagas.Interfaces;

public interface IProductServiceClient
{
    Task ApplyPurchaseBatchUpdateAsync(ProductPurchaseBatchUpdateRequest request, CancellationToken cancellationToken = default);
    Task RollbackPurchaseBatchUpdateAsync(ProductPurchaseBatchRollbackRequest request, CancellationToken cancellationToken = default);
}