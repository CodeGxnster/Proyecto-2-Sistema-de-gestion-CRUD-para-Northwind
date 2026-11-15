namespace Northwind.Comun.Registro;

/// <summary>
/// Resuelve rutas relativas tomando como base el directorio del ejecutable.
/// </summary>
public static class RutasAplicacion
{
    /// <summary>
    /// Convierte una ruta relativa en una ruta absoluta dentro del directorio de ejecucion.
    /// </summary>
    public static string ResolverRuta(string ruta)
    {
        if (string.IsNullOrWhiteSpace(ruta))
        {
            return AppContext.BaseDirectory;
        }

        return Path.IsPathRooted(ruta)
            ? ruta
            : Path.Combine(AppContext.BaseDirectory, ruta.Replace('/', Path.DirectorySeparatorChar));
    }

    /// <summary>
    /// Garantiza que exista el directorio indicado y devuelve su ruta absoluta.
    /// </summary>
    public static string AsegurarDirectorio(string ruta)
    {
        var rutaCompleta = ResolverRuta(ruta);
        Directory.CreateDirectory(rutaCompleta);
        return rutaCompleta;
    }
}
