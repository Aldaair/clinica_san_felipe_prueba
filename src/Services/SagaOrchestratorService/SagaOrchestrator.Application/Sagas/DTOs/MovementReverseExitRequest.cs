namespace SagaOrchestrator.Application.Sagas.DTOs;

public sealed class MovementReverseExitRequest
{
    public int IdDocumentoOrigen { get; set; }
    public int IdTipoMovimiento { get; set; } = 2;
}