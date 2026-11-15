namespace Northwind.Comun.Configuracion;

/// <summary>
/// Configuracion general de la aplicacion leida desde appsettings.json.
/// </summary>
public class AplicacionSettings
{
    public const string Seccion = "Aplicacion";

    public string Nombre { get; set; } = "Northwind Gestion CRUD";

    public string Version { get; set; } = "1.0.0";

    public string Entorno { get; set; } = "Desarrollo";
}
