namespace SagaOrchestrator.Application.Sagas.DTOs;

public sealed class MovementEntryRequest
{
    public int IdDocumentoOrigen { get; set; }
    public int IdTipoMovimiento { get; set; } = 1;
    public List<MovementEntryItemRequest> Items { get; set; } = new();
}

public sealed class MovementEntryItemRequest
{
    public int IdProducto { get; set; }
    public decimal Cantidad { get; set; }
}