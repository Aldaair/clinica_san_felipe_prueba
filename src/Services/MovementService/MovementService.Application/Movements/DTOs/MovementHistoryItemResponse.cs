namespace MovementService.Application.Movements.DTOs;

public sealed record MovementHistoryItemResponse(
    DateTime FechaRegistro,
    string TipoMovimiento,
    decimal Cantidad,
    int IdDocumentoOrigen,
    bool IsCompensation
);