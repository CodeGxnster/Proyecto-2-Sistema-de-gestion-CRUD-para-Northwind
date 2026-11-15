namespace Northwind.Entidades;

/// <summary>
/// Resultado del reporte de suplidores con mayor valor de inventario.
/// </summary>
public class ReporteInventarioSuplidorDTO
{
    public string Suplidor { get; set; } = string.Empty;

    public int CantidadProductos { get; set; }

    public int UnidadesEnInventario { get; set; }

    public decimal ValorInventario { get; set; }
}
