using MovementService.Application.Movements.DTOs;
using MovementService.Application.Movements.Interfaces;
using MovementService.Domain.Entities;
using MovementService.Domain.Enums;

namespace MovementService.Application.Movements.Services;

public sealed class MovementAppService : IMovementAppService
{
    private readonly IMovementRepository _movementRepository;

    public MovementAppService(IMovementRepository movementRepository)
    {
        _movementRepository = movementRepository;
    }

    public async Task<MovementResponse> RegisterEntryAsync(MovementEntryRequest request, CancellationToken cancellationToken = default)
    {
        ValidateItems(request.Items);

        var movement = new MovimientoCab(
            DateTime.UtcNow,
            MovementType.Entry,
            request.IdDocumentoOrigen,
            isCompensation: false);

        foreach (var item in request.Items)
        {
            movement.AddDetail(item.IdProducto, item.Cantidad);
        }

        await _movementRepository.AddMovementAsync(movement, cancellationToken);
        await _movementRepository.SaveChangesAsync(cancellationToken);

        return new MovementResponse(
            movement.Id,
            movement.IdDocumentoOrigen,
            movement.IdTipoMovimiento.ToString(),
            movement.IsCompensation,
            "Movimiento de entrada registrado correctamente.");
    }

    public async Task<MovementResponse> RegisterExitAsync(MovementExitRequest request, CancellationToken cancellationToken = default)
    {
        ValidateItems(request.Items);

        foreach (var item in request.Items)
        {
            var stock = await _movementRepository.GetStockByProductIdAsync(item.IdProducto, cancellationToken);
            if (item.Cantidad > stock)
            {
                throw new InvalidOperationException(
                    $"Stock insuficiente para el producto {item.IdProducto}. Stock actual: {stock}.");
            }
        }

        var movement = new MovimientoCab(
            DateTime.UtcNow,
            MovementType.Exit,
            request.IdDocumentoOrigen,
            isCompensation: false);

        foreach (var item in request.Items)
        {
            movement.AddDetail(item.IdProducto, item.Cantidad);
        }

        await _movementRepository.AddMovementAsync(movement, cancellationToken);
        await _movementRepository.SaveChangesAsync(cancellationToken);

        return new MovementResponse(
            movement.Id,
            movement.IdDocumentoOrigen,
            movement.IdTipoMovimiento.ToString(),
            movement.IsCompensation,
            "Movimiento de salida registrado correctamente.");
    }

    public async Task<MovementResponse> ReverseEntryAsync(MovementReverseEntryRequest request, CancellationToken cancellationToken = default)
    {
        var alreadyCompensated = await _movementRepository.ExistsCompensationAsync(
            request.IdDocumentoOrigen,
            MovementType.Entry,
            MovementType.Exit,
            cancellationToken);

        if (alreadyCompensated)
        {
            return new MovementResponse(
                0,
                request.IdDocumentoOrigen,
                MovementType.Exit.ToString(),
                true,
                "La reversa ya fue aplicada previamente.");
        }

        var sourceMovements = await _movementRepository.GetMovementsByDocumentAndTypeAsync(
            request.IdDocumentoOrigen,
            MovementType.Entry,
            cancellationToken);

        if (sourceMovements.Count == 0)
            throw new InvalidOperationException("No existen movimientos de entrada para compensar.");

        var groupedItems = sourceMovements
            .SelectMany(x => x.Details)
            .GroupBy(x => x.IdProducto)
            .Select(g => new { IdProducto = g.Key, Cantidad = g.Sum(x => x.Cantidad) })
            .ToList();

        var compensationMovement = new MovimientoCab(
            DateTime.UtcNow,
            MovementType.Exit,
            request.IdDocumentoOrigen,
            isCompensation: true);

        foreach (var item in groupedItems)
        {
            compensationMovement.AddDetail(item.IdProducto, item.Cantidad);
        }

        await _movementRepository.AddMovementAsync(compensationMovement, cancellationToken);
        await _movementRepository.AddCompensationLogAsync(
            new MovementCompensationLog(
                request.IdDocumentoOrigen,
                MovementType.Entry,
                MovementType.Exit),
            cancellationToken);

        await _movementRepository.SaveChangesAsync(cancellationToken);

        return new MovementResponse(
            compensationMovement.Id,
            compensationMovement.IdDocumentoOrigen,
            compensationMovement.IdTipoMovimiento.ToString(),
            compensationMovement.IsCompensation,
            "Reversa de entrada aplicada correctamente.");
    }

    public async Task<StockResponse> GetStockByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var stock = await _movementRepository.GetStockByProductIdAsync(productId, cancellationToken);
        return new StockResponse(productId, stock);
    }

    public async Task<IReadOnlyList<StockResponse>> GetCurrentStockAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _movementRepository.GetCurrentStockAsync(cancellationToken);
        return rows.Select(x => new StockResponse(x.IdProducto, x.StockActual)).ToList();
    }

    public async Task<IReadOnlyList<MovementHistoryItemResponse>> GetHistoryByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var movements = await _movementRepository.GetHistoryByProductIdAsync(productId, cancellationToken);

        return movements
            .SelectMany(m => m.Details
                .Where(d => d.IdProducto == productId)
                .Select(d => new MovementHistoryItemResponse(
                    m.FecRegistro,
                    m.IdTipoMovimiento.ToString(),
                    d.Cantidad,
                    m.IdDocumentoOrigen,
                    m.IsCompensation)))
            .OrderByDescending(x => x.FechaRegistro)
            .ToList();
    }

    private static void ValidateItems(IEnumerable<MovementItemRequest> items)
    {
        if (items is null || !items.Any())
            throw new InvalidOperationException("El movimiento debe contener al menos un item.");

        foreach (var item in items)
        {
            if (item.IdProducto <= 0)
                throw new InvalidOperationException("IdProducto inválido.");

            if (item.Cantidad <= 0)
                throw new InvalidOperationException("La cantidad debe ser mayor a cero.");
        }
    }
    public async Task<MovementResponse> ReverseExitAsync(MovementReverseExitRequest request, CancellationToken cancellationToken = default)
    {
        var alreadyCompensated = await _movementRepository.ExistsCompensationAsync(
            request.IdDocumentoOrigen,
            MovementType.Exit,
            MovementType.Entry,
            cancellationToken);

        if (alreadyCompensated)
        {
            return new MovementResponse(
                0,
                request.IdDocumentoOrigen,
                MovementType.Entry.ToString(),
                true,
                "La reversa ya fue aplicada previamente.");
        }

        var sourceMovements = await _movementRepository.GetMovementsByDocumentAndTypeAsync(
            request.IdDocumentoOrigen,
            MovementType.Exit,
            cancellationToken);

        if (sourceMovements.Count == 0)
            throw new InvalidOperationException("No existen movimientos de salida para compensar.");

        var groupedItems = sourceMovements
            .SelectMany(x => x.Details)
            .GroupBy(x => x.IdProducto)
            .Select(g => new { IdProducto = g.Key, Cantidad = g.Sum(x => x.Cantidad) })
            .ToList();

        var compensationMovement = new MovimientoCab(
            DateTime.UtcNow,
            MovementType.Entry,
            request.IdDocumentoOrigen,
            isCompensation: true);

        foreach (var item in groupedItems)
        {
            compensationMovement.AddDetail(item.IdProducto, item.Cantidad);
        }

        await _movementRepository.AddMovementAsync(compensationMovement, cancellationToken);
        await _movementRepository.AddCompensationLogAsync(
            new MovementCompensationLog(
                request.IdDocumentoOrigen,
                MovementType.Exit,
                MovementType.Entry),
            cancellationToken);

        await _movementRepository.SaveChangesAsync(cancellationToken);

        return new MovementResponse(
            compensationMovement.Id,
            compensationMovement.IdDocumentoOrigen,
            compensationMovement.IdTipoMovimiento.ToString(),
            compensationMovement.IsCompensation,
            "Reversa de salida aplicada correctamente.");
    }
}