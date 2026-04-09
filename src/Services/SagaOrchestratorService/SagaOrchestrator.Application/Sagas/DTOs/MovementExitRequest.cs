namespace SagaOrchestrator.Application.Sagas.DTOs;

public sealed class MovementExitRequest
{
    public int IdDocumentoOrigen { get; set; }
    public int IdTipoMovimiento { get; set; } = 2;
    public List<MovementExitItemRequest> Items { get; set; } = new();
}

public sealed class MovementExitItemRequest
{
    public int IdProducto { get; set; }
    public decimal Cantidad { get; set; }
}