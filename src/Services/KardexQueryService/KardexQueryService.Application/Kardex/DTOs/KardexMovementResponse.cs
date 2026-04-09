namespace KardexQueryService.Application.Kardex.DTOs;

public sealed record KardexMovementResponse(
    DateTime FechaRegistro,
    string TipoMovimiento,
    decimal Cantidad
);