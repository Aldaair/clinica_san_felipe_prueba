namespace SagaOrchestrator.Application.Sagas.DTOs;

public sealed class MovementReverseEntryRequest
{
    public int IdDocumentoOrigen { get; set; }
    public int IdTipoMovimiento { get; set; } = 1;
}