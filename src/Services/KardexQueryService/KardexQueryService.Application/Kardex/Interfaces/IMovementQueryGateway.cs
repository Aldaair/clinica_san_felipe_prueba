using KardexQueryService.Application.Kardex.DTOs;

namespace KardexQueryService.Application.Kardex.Interfaces;

public interface IMovementQueryGateway
{
    Task<IReadOnlyList<StockDto>> GetCurrentStockAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KardexMovementResponse>> GetHistoryByProductIdAsync(int productId, CancellationToken cancellationToken = default);
}