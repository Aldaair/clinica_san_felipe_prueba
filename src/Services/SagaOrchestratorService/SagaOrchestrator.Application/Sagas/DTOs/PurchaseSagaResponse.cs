namespace SagaOrchestrator.Application.Sagas.DTOs;

public sealed record PurchaseSagaResponse(
    int SagaId,
    Guid CorrelationId,
    string SagaType,
    string Status,
    string CurrentStep,
    string? ErrorMessage,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);