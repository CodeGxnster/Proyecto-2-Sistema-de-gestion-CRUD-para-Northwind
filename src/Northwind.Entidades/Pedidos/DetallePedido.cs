namespace Northwind.Entidades;

/// <summary>
/// Detalle de un pedido de Northwind.
/// </summary>
public class DetallePedido
{
    public int PedidoId { get; set; }

    public int ProductoId { get; set; }

    public decimal PrecioUnitario { get; set; }

    public short Cantidad { get; set; }

    public float Descuento { get; set; }

    public Pedido? Pedido { get; set; }

    public Producto? Producto { get; set; }
}
