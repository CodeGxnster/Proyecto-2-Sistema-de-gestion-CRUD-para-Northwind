using Northwind.Entidades;

namespace Northwind.LogicaNegocio.Servicios;

/// <summary>
/// Operaciones de negocio de los reportes analiticos.
/// </summary>
public interface IReporteServicio
{
    IReadOnlyList<ReporteTopProductoDTO> ObtenerTopProductosVendidos();

    IReadOnlyList<ReporteVentasCategoriaDTO> ObtenerVentasPorCategoria(DateTime desde, DateTime hasta);

    IReadOnlyList<ReporteInventarioSuplidorDTO> ObtenerSuplidoresMayorValorInventario();
}
