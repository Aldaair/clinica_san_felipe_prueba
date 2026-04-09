namespace SagaOrchestrator.Domain.Enums;

public enum SagaStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Failed = 4,
    Compensated = 5
}