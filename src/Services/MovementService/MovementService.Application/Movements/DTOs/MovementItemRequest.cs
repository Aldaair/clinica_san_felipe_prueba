using System.ComponentModel.DataAnnotations;

namespace MovementService.Application.Movements.DTOs;

public sealed class MovementItemRequest
{
    [Range(1, int.MaxValue)]
    public int IdProducto { get; set; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Cantidad { get; set; }
}