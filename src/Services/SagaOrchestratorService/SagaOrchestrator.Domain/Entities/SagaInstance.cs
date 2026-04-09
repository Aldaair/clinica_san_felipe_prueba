using SagaOrchestrator.Domain.Enums;

namespace SagaOrchestrator.Domain.Entities;

public sealed class SagaInstance
{
    public int Id { get; private set; }
    public Guid CorrelationId { get; private set; }
    public SagaType SagaType { get; private set; }
    public SagaStatus Status { get; private set; }
    public string CurrentStep { get; private set; } = string.Empty;
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public List<SagaStep> Steps { get; private set; } = new();

    private SagaInstance() { }

    public SagaInstance(SagaType sagaType)
    {
        CorrelationId = Guid.NewGuid();
        SagaType = sagaType;
        Status = SagaStatus.Pending;
        CurrentStep = "Created";
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkInProgress(string currentStep)
    {
        Status = SagaStatus.InProgress;
        CurrentStep = currentStep;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkCompleted(string currentStep)
    {
        Status = SagaStatus.Completed;
        CurrentStep = currentStep;
        ErrorMessage = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkFailed(string currentStep, string errorMessage)
    {
        Status = SagaStatus.Failed;
        CurrentStep = currentStep;
        ErrorMessage = errorMessage;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkCompensated(string currentStep)
    {
        Status = SagaStatus.Compensated;
        CurrentStep = currentStep;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void AddStep(
        string stepName,
        SagaStepStatus status,
        string? requestPayload,
        string? responsePayload,
        string? errorMessage,
        bool isCompensation = false)
    {
        Steps.Add(new SagaStep(
            stepName,
            status,
            requestPayload,
            responsePayload,
            errorMessage,
            isCompensation));

        UpdatedAtUtc = DateTime.UtcNow;
    }
}