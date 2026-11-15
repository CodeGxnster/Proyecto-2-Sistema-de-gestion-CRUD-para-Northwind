using Northwind.Entidades;

namespace Northwind.AccesoDatos.Repositorios;

/// <summary>
/// Consultas analiticas de solo lectura sobre Northwind.
/// </summary>
public interface IReporteRepositorio
{
    IReadOnlyList<ReporteTopProductoDTO> ObtenerTopProductosVendidos(int top);

    IReadOnlyList<ReporteVentasCategoriaDTO> ObtenerVentasPorCategoria(DateTime desde, DateTime hasta);

    IReadOnlyList<ReporteInventarioSuplidorDTO> ObtenerSuplidoresMayorValorInventario(int top);
}
