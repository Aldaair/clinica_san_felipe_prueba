using Microsoft.EntityFrameworkCore;
using MovementService.Application.Movements.Interfaces;
using MovementService.Domain.Entities;
using MovementService.Domain.Enums;
using MovementService.Infrastructure.Persistence;

namespace MovementService.Infrastructure.Movements;

public sealed class MovementRepository : IMovementRepository
{
    private readonly MovementDbContext _dbContext;

    public MovementRepository(MovementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddMovementAsync(MovimientoCab movement, CancellationToken cancellationToken = default)
    {
        await _dbContext.MovimientoCabs.AddAsync(movement, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MovimientoCab>> GetMovementsByDocumentAndTypeAsync(
        int idDocumentoOrigen,
        MovementType movementType,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.MovimientoCabs
            .Include(x => x.Details)
            .Where(x => x.IdDocumentoOrigen == idDocumentoOrigen && x.IdTipoMovimiento == movementType)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsCompensationAsync(
        int idDocumentoOrigen,
        MovementType originalType,
        MovementType compensationType,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.MovementCompensationLogs.AnyAsync(
            x => x.IdDocumentoOrigen == idDocumentoOrigen
              && x.OriginalMovementType == originalType
              && x.CompensationMovementType == compensationType,
            cancellationToken);
    }

    public async Task AddCompensationLogAsync(MovementCompensationLog log, CancellationToken cancellationToken = default)
    {
        await _dbContext.MovementCompensationLogs.AddAsync(log, cancellationToken);
    }

    public async Task<decimal> GetStockByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var rows = await _dbContext.MovimientoDets
            .Where(x => x.IdProducto == productId)
            .Join(
                _dbContext.MovimientoCabs,
                det => det.IdMovimientoCab,
                cab => cab.Id,
                (det, cab) => new
                {
                    det.Cantidad,
                    cab.IdTipoMovimiento
                })
            .ToListAsync(cancellationToken);

        var stock = rows.Sum(x => x.IdTipoMovimiento == MovementType.Entry ? x.Cantidad : -x.Cantidad);
        return stock;
    }

    public async Task<IReadOnlyList<StockResponseProjection>> GetCurrentStockAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _dbContext.MovimientoDets
            .Join(
                _dbContext.MovimientoCabs,
                det => det.IdMovimientoCab,
                cab => cab.Id,
                (det, cab) => new
                {
                    det.IdProducto,
                    det.Cantidad,
                    cab.IdTipoMovimiento
                })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(x => x.IdProducto)
            .Select(g => new StockResponseProjection(
                g.Key,
                g.Sum(x => x.IdTipoMovimiento == MovementType.Entry ? x.Cantidad : -x.Cantidad)))
            .OrderBy(x => x.IdProducto)
            .ToList();
    }

    public async Task<IReadOnlyList<MovimientoCab>> GetHistoryByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MovimientoCabs
            .AsNoTracking()
            .Include(x => x.Details)
            .Where(x => x.Details.Any(d => d.IdProducto == productId))
            .OrderByDescending(x => x.FecRegistro)
            .ToListAsync(cancellationToken);
    }
}