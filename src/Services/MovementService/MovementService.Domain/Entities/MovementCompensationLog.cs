using MovementService.Domain.Enums;

namespace MovementService.Domain.Entities;

public sealed class MovementCompensationLog
{
    public int Id { get; private set; }
    public int IdDocumentoOrigen { get; private set; }
    public MovementType OriginalMovementType { get; private set; }
    public MovementType CompensationMovementType { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private MovementCompensationLog() { }

    public MovementCompensationLog(
        int idDocumentoOrigen,
        MovementType originalMovementType,
        MovementType compensationMovementType)
    {
        IdDocumentoOrigen = idDocumentoOrigen;
        OriginalMovementType = originalMovementType;
        CompensationMovementType = compensationMovementType;
        CreatedAtUtc = DateTime.UtcNow;
    }
}