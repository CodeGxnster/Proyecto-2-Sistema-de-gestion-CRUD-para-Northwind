using Microsoft.Extensions.Options;
using Northwind.AccesoDatos.Repositorios;
using Northwind.Comun.Configuracion;
using Northwind.Entidades;
using Serilog;

namespace Northwind.LogicaNegocio.Servicios;

/// <summary>
/// Servicio de reportes con validacion de parametros y registro de eventos.
/// </summary>
public class ReporteServicio : IReporteServicio
{
    private readonly IReporteRepositorio _repositorio;
    private readonly ReportesSettings _ajustes;
    private readonly ILogger _logger;

    public ReporteServicio(
        IReporteRepositorio repositorio,
        IOptions<ReportesSettings> ajustes,
        ILogger logger)
    {
        _repositorio = repositorio;
        _ajustes = ajustes.Value;
        _logger = logger.ForContext<ReporteServicio>();
    }

    public IReadOnlyList<ReporteTopProductoDTO> ObtenerTopProductosVendidos()
    {
        var top = Math.Max(1, _ajustes.TopProductos);
        var datos = _repositorio.ObtenerTopProductosVendidos(top);

        _logger.Information(
            "Reporte de top productos generado con {Cantidad} resultados (top {Top})",
            datos.Count,
            top);

        return datos;
    }

    public IReadOnlyList<ReporteVentasCategoriaDTO> ObtenerVentasPorCategoria(DateTime desde, DateTime hasta)
    {
        if (desde.Date > hasta.Date)
        {
            _logger.Warning(
                "Rango de fechas invalido para el reporte de ventas: {Desde} - {Hasta}",
                desde,
                hasta);
            return Array.Empty<ReporteVentasCategoriaDTO>();
        }

        var datos = _repositorio.ObtenerVentasPorCategoria(desde, hasta);

        _logger.Information(
            "Reporte de ventas por categoria generado con {Cantidad} resultados entre {Desde} y {Hasta}",
            datos.Count,
            desde,
            hasta);

        return datos;
    }

    public IReadOnlyList<ReporteInventarioSuplidorDTO> ObtenerSuplidoresMayorValorInventario()
    {
        var top = Math.Max(1, _ajustes.TopSuplidores);
        var datos = _repositorio.ObtenerSuplidoresMayorValorInventario(top);

        _logger.Information(
            "Reporte de inventario por suplidor generado con {Cantidad} resultados (top {Top})",
            datos.Count,
            top);

        return datos;
    }
}
