using System.ComponentModel.DataAnnotations;

namespace SagaOrchestrator.Application.Sagas.DTOs;

public sealed class PurchaseSagaItemRequest
{
    [Range(1, int.MaxValue)]
    public int IdProducto { get; set; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Cantidad { get; set; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Precio { get; set; }
}