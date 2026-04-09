namespace SagaOrchestrator.Domain.Enums;

public enum SagaStepStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Compensated = 4
}