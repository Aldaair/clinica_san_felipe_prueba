using SagaOrchestrator.Domain.Enums;

namespace SagaOrchestrator.Domain.Entities;

public sealed class SagaStep
{
    public int Id { get; private set; }
    public int SagaInstanceId { get; private set; }
    public string StepName { get; private set; } = string.Empty;
    public SagaStepStatus Status { get; private set; }
    public string? RequestPayload { get; private set; }
    public string? ResponsePayload { get; private set; }
    public string? ErrorMessage { get; private set; }
    public bool IsCompensation { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private SagaStep() { }

    public SagaStep(
        string stepName,
        SagaStepStatus status,
        string? requestPayload,
        string? responsePayload,
        string? errorMessage,
        bool isCompensation)
    {
        StepName = stepName;
        Status = status;
        RequestPayload = requestPayload;
        ResponsePayload = responsePayload;
        ErrorMessage = errorMessage;
        IsCompensation = isCompensation;
        CreatedAtUtc = DateTime.UtcNow;
    }
}