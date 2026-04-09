namespace MovementService.Domain.Entities;

public sealed class MovimientoDet
{
    public int Id { get; private set; }
    public int IdMovimientoCab { get; private set; }
    public int IdProducto { get; private set; }
    public decimal Cantidad { get; private set; }

    private MovimientoDet() { }

    internal MovimientoDet(int idProducto, decimal cantidad)
    {
        IdProducto = idProducto;
        Cantidad = cantidad;
    }
}