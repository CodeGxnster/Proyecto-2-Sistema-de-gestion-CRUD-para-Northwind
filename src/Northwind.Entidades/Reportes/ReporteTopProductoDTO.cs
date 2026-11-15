namespace Northwind.Entidades;

/// <summary>
/// Resultado del reporte de productos mas vendidos.
/// </summary>
public class ReporteTopProductoDTO
{
    public int ProductoId { get; set; }

    public string NombreProducto { get; set; } = string.Empty;

    public string NombreCategoria { get; set; } = string.Empty;

    public int UnidadesVendidas { get; set; }

    public decimal TotalVendido { get; set; }
}
