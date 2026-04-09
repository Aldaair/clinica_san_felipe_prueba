using MovementService.Domain.Enums;

namespace MovementService.Domain.Entities;

public sealed class MovimientoCab
{
    public int Id { get; private set; }
    public DateTime FecRegistro { get; private set; }
    public MovementType IdTipoMovimiento { get; private set; }
    public int IdDocumentoOrigen { get; private set; }
    public bool IsCompensation { get; private set; }

    public List<MovimientoDet> Details { get; private set; } = new();

    private MovimientoCab() { }

    public MovimientoCab(DateTime fecRegistro, MovementType idTipoMovimiento, int idDocumentoOrigen, bool isCompensation = false)
    {
        FecRegistro = fecRegistro;
        IdTipoMovimiento = idTipoMovimiento;
        IdDocumentoOrigen = idDocumentoOrigen;
        IsCompensation = isCompensation;
    }

    public void AddDetail(int idProducto, decimal cantidad)
    {
        Details.Add(new MovimientoDet(idProducto, cantidad));
    }
}