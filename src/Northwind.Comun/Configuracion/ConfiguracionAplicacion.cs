using Microsoft.Extensions.Configuration;

namespace Northwind.Comun.Configuracion;

/// <summary>
/// Carga la configuracion de appsettings.json y variables de entorno.
/// </summary>
public static class ConfiguracionAplicacion
{
    /// <summary>
    /// Construye la configuracion desde el directorio de ejecucion.
    /// </summary>
    public static IConfiguration Cargar()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables("NORTHWIND_")
            .Build();
    }

    /// <summary>
    /// Obtiene la cadena de conexion desde ConnectionStrings o BaseDatos.
    /// </summary>
    public static string ObtenerCadenaConexion(IConfiguration configuracion)
    {
        var desdeConnectionStrings = configuracion.GetConnectionString("Northwind");
        if (!string.IsNullOrWhiteSpace(desdeConnectionStrings))
        {
            return desdeConnectionStrings.Trim();
        }

        return configuracion[$"{BaseDatosSettings.Seccion}:{nameof(BaseDatosSettings.CadenaConexion)}"]?.Trim()
            ?? string.Empty;
    }
}
