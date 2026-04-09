using System.ComponentModel.DataAnnotations;

namespace MovementService.Application.Movements.DTOs;

public sealed class MovementReverseEntryRequest
{
    [Range(1, int.MaxValue)]
    public int IdDocumentoOrigen { get; set; }

    [Range(1, 1)]
    public int IdTipoMovimiento { get; set; } = 1;
}