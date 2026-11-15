namespace Northwind.Comun.Configuracion;

/// <summary>
/// Parametros de los reportes analiticos y su exportacion.
/// </summary>
public class ReportesSettings
{
    public const string Seccion = "Reportes";

    public int TopProductos { get; set; } = 10;

    public int TopSuplidores { get; set; } = 10;

    public string CarpetaExportacion { get; set; } = "reportes";
}
