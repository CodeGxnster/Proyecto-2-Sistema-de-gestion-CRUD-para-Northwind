namespace Northwind.Entidades;

/// <summary>
/// Pedido registrado en Northwind.
/// </summary>
public class Pedido
{
    public int PedidoId { get; set; }

    public DateTime? FechaPedido { get; set; }

    public DateTime? FechaRequerida { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
}
