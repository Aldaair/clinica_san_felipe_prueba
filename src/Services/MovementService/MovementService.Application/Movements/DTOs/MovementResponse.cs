namespace MovementService.Application.Movements.DTOs;

public sealed record MovementResponse(
    int IdMovimientoCab,
    int IdDocumentoOrigen,
    string TipoMovimiento,
    bool IsCompensation,
    string Message
);