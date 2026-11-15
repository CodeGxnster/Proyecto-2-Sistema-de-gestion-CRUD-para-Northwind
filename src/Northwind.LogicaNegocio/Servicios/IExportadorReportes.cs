namespace Northwind.LogicaNegocio.Servicios;

/// <summary>
/// Exporta reportes tabulares a formatos de archivo.
/// </summary>
public interface IExportadorReportes
{
    void ExportarCsv(
        string ruta,
        IReadOnlyList<string> encabezados,
        IReadOnlyList<IReadOnlyList<object?>> filas);

    void ExportarExcel(
        string ruta,
        string nombreHoja,
        IReadOnlyList<string> encabezados,
        IReadOnlyList<IReadOnlyList<object?>> filas);
}
