using SagaOrchestrator.Application.Sagas.DTOs;

namespace SagaOrchestrator.Application.Sagas.Interfaces;

public interface IMovementServiceClient
{
    Task RegisterEntryAsync(MovementEntryRequest request, CancellationToken cancellationToken = default);
    Task ReverseEntryAsync(MovementReverseEntryRequest request, CancellationToken cancellationToken = default);

    Task<ProductStockResponse> GetStockByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task RegisterExitAsync(MovementExitRequest request, CancellationToken cancellationToken = default);
    Task ReverseExitAsync(MovementReverseExitRequest request, CancellationToken cancellationToken = default);
}