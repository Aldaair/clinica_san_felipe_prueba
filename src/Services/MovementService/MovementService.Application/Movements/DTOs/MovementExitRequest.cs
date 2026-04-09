using System.ComponentModel.DataAnnotations;

namespace MovementService.Application.Movements.DTOs;

public sealed class MovementExitRequest
{
    [Range(1, int.MaxValue)]
    public int IdDocumentoOrigen { get; set; }

    [Range(2, 2)]
    public int IdTipoMovimiento { get; set; } = 2;

    [Required]
    [MinLength(1)]
    public List<MovementItemRequest> Items { get; set; } = new();
}