namespace SagaOrchestrator.Application.Sagas.DTOs;

public sealed record SaleSagaResponse(
    int SagaId,
    Guid CorrelationId,
    string SagaType,
    string Status,
    string CurrentStep,
    string? ErrorMessage,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);