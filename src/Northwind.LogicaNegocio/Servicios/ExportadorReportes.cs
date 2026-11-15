using System.Globalization;
using System.Text;
using ClosedXML.Excel;

namespace Northwind.LogicaNegocio.Servicios;

/// <summary>
/// Exportador de reportes a CSV y a Excel mediante ClosedXML.
/// </summary>
public class ExportadorReportes : IExportadorReportes
{
    public void ExportarCsv(
        string ruta,
        IReadOnlyList<string> encabezados,
        IReadOnlyList<IReadOnlyList<object?>> filas)
    {
        AsegurarDirectorio(ruta);

        using var escritor = new StreamWriter(ruta, false, new UTF8Encoding(true));

        escritor.WriteLine(string.Join(';', encabezados.Select(EscaparCsv)));
        foreach (var fila in filas)
        {
            escritor.WriteLine(string.Join(';', fila.Select(valor => EscaparCsv(Formatear(valor)))));
        }
    }

    public void ExportarExcel(
        string ruta,
        string nombreHoja,
        IReadOnlyList<string> encabezados,
        IReadOnlyList<IReadOnlyList<object?>> filas)
    {
        AsegurarDirectorio(ruta);

        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add(string.IsNullOrWhiteSpace(nombreHoja) ? "Reporte" : nombreHoja);

        for (var columna = 0; columna < encabezados.Count; columna++)
        {
            hoja.Cell(1, columna + 1).Value = encabezados[columna];
        }

        var rangoEncabezado = hoja.Range(1, 1, 1, encabezados.Count);
        rangoEncabezado.Style.Font.Bold = true;
        rangoEncabezado.Style.Font.FontColor = XLColor.White;
        rangoEncabezado.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E1E1E");
        rangoEncabezado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        for (var indiceFila = 0; indiceFila < filas.Count; indiceFila++)
        {
            var fila = filas[indiceFila];
            for (var indiceColumna = 0; indiceColumna < fila.Count; indiceColumna++)
            {
                var celda = hoja.Cell(indiceFila + 2, indiceColumna + 1);
                AsignarValor(celda, fila[indiceColumna]);
            }
        }

        hoja.SheetView.FreezeRows(1);
        hoja.Columns().AdjustToContents();
        libro.SaveAs(ruta);
    }

    private static void AsegurarDirectorio(string ruta)
    {
        var directorio = Path.GetDirectoryName(ruta);
        if (!string.IsNullOrWhiteSpace(directorio))
        {
            Directory.CreateDirectory(directorio);
        }
    }

    private static void AsignarValor(IXLCell celda, object? valor)
    {
        switch (valor)
        {
            case null:
                celda.Value = string.Empty;
                break;
            case DateTime fecha:
                celda.Value = fecha;
                celda.Style.DateFormat.Format = "yyyy-mm-dd hh:mm";
                break;
            case decimal numero:
                celda.Value = numero;
                celda.Style.NumberFormat.Format = "#,##0.00";
                break;
            case int entero:
                celda.Value = entero;
                break;
            case short enteroCorto:
                celda.Value = (int)enteroCorto;
                break;
            case double doble:
                celda.Value = doble;
                celda.Style.NumberFormat.Format = "#,##0.00";
                break;
            case float flotante:
                celda.Value = (double)flotante;
                celda.Style.NumberFormat.Format = "#,##0.00";
                break;
            case bool booleano:
                celda.Value = booleano;
                break;
            default:
                celda.Value = valor.ToString() ?? string.Empty;
                break;
        }
    }

    private static string Formatear(object? valor)
    {
        return valor switch
        {
            null => string.Empty,
            DateTime fecha => fecha.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
            decimal numero => numero.ToString("0.00", CultureInfo.InvariantCulture),
            double numero => numero.ToString("0.00", CultureInfo.InvariantCulture),
            float numero => numero.ToString("0.00", CultureInfo.InvariantCulture),
            _ => valor.ToString() ?? string.Empty
        };
    }

    private static string EscaparCsv(string valor)
    {
        if (valor.Contains(';') || valor.Contains('"') || valor.Contains('\n') || valor.Contains('\r'))
        {
            return $"\"{valor.Replace("\"", "\"\"")}\"";
        }

        return valor;
    }
}
