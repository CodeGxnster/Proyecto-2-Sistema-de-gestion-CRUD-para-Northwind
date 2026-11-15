namespace Northwind.Comun.Configuracion;

/// <summary>
/// Configuracion del registro estructurado con Serilog.
/// </summary>
public class RegistroSettings
{
    public const string Seccion = "Registro";

    public string NivelMinimo { get; set; } = "Information";

    public string RutaArchivo { get; set; } = "logs/app-.log";

    public int DiasRetencion { get; set; } = 31;
}
