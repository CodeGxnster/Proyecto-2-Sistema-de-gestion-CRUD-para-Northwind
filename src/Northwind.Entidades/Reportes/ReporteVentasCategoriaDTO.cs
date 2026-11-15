namespace Northwind.Entidades;

/// <summary>
/// Resultado del reporte de ventas por categoria en un rango de fechas.
/// </summary>
public class ReporteVentasCategoriaDTO
{
    public string Categoria { get; set; } = string.Empty;

    public int UnidadesVendidas { get; set; }

    public decimal TotalVentas { get; set; }

    public decimal Porcentaje { get; set; }
}
