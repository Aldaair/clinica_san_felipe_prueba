using MovementService.Application.Movements.DTOs;

namespace MovementService.Application.Movements.Interfaces;

public interface IMovementAppService
{
    Task<MovementResponse> RegisterEntryAsync(MovementEntryRequest request, CancellationToken cancellationToken = default);
    Task<MovementResponse> RegisterExitAsync(MovementExitRequest request, CancellationToken cancellationToken = default);
    Task<MovementResponse> ReverseEntryAsync(MovementReverseEntryRequest request, CancellationToken cancellationToken = default);

    Task<StockResponse> GetStockByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockResponse>> GetCurrentStockAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MovementHistoryItemResponse>> GetHistoryByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<MovementResponse> ReverseExitAsync(MovementReverseExitRequest request, CancellationToken cancellationToken = default);
}