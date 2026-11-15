using Microsoft.Extensions.Configuration;
using Northwind.Comun.Configuracion;
using Serilog;
using Serilog.Events;

namespace Northwind.Comun.Registro;

/// <summary>
/// Fabrica del registro estructurado de la aplicacion con salida a consola y archivo diario.
/// </summary>
public static class RegistroAplicacion
{
    private const string PlantillaSalida =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// Crea el logger global de Serilog a partir de la configuracion.
    /// </summary>
    public static ILogger Crear(IConfiguration configuracion)
    {
        var nivelTexto = configuracion[$"{RegistroSettings.Seccion}:{nameof(RegistroSettings.NivelMinimo)}"] ?? "Information";
        var rutaArchivo = configuracion[$"{RegistroSettings.Seccion}:{nameof(RegistroSettings.RutaArchivo)}"] ?? "logs/app-.log";
        var retencion = int.TryParse(
            configuracion[$"{RegistroSettings.Seccion}:{nameof(RegistroSettings.DiasRetencion)}"],
            out var dias) ? dias : 31;

        var nivel = Enum.TryParse<LogEventLevel>(nivelTexto, ignoreCase: true, out var nivelConfigurado)
            ? nivelConfigurado
            : LogEventLevel.Information;

        var rutaCompleta = RutasAplicacion.ResolverRuta(rutaArchivo);
        var directorio = Path.GetDirectoryName(rutaCompleta);
        if (!string.IsNullOrWhiteSpace(directorio))
        {
            Directory.CreateDirectory(directorio);
        }

        return new LoggerConfiguration()
            .MinimumLevel.Is(nivel)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Aplicacion", "Northwind")
            .WriteTo.Console(outputTemplate: PlantillaSalida)
            .WriteTo.File(
                rutaCompleta,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: retencion,
                shared: false,
                outputTemplate: PlantillaSalida)
            .CreateLogger();
    }
}
