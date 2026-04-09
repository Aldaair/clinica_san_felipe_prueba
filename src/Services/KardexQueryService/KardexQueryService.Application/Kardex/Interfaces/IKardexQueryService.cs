using KardexQueryService.Application.Kardex.DTOs;

namespace KardexQueryService.Application.Kardex.Interfaces;

public interface IKardexQueryService
{
    Task<IReadOnlyList<KardexResponse>> GetKardexAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KardexMovementResponse>> GetProductMovementsAsync(int productId, CancellationToken cancellationToken = default);
}