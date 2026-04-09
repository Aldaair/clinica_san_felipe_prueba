using MovementService.Domain.Entities;
using MovementService.Domain.Enums;

namespace MovementService.Application.Movements.Interfaces;

public interface IMovementRepository
{
    Task AddMovementAsync(MovimientoCab movement, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MovimientoCab>> GetMovementsByDocumentAndTypeAsync(
        int idDocumentoOrigen,
        MovementType movementType,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsCompensationAsync(
        int idDocumentoOrigen,
        MovementType originalType,
        MovementType compensationType,
        CancellationToken cancellationToken = default);

    Task AddCompensationLogAsync(MovementCompensationLog log, CancellationToken cancellationToken = default);

    Task<decimal> GetStockByProductIdAsync(int productId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StockResponseProjection>> GetCurrentStockAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MovimientoCab>> GetHistoryByProductIdAsync(int productId, CancellationToken cancellationToken = default);
}

public sealed record StockResponseProjection(int IdProducto, decimal StockActual);