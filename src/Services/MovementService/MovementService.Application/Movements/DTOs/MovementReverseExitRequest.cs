using System.ComponentModel.DataAnnotations;

namespace MovementService.Application.Movements.DTOs;

public sealed class MovementReverseExitRequest
{
    [Range(1, int.MaxValue)]
    public int IdDocumentoOrigen { get; set; }

    [Range(2, 2)]
    public int IdTipoMovimiento { get; set; } = 2;
}