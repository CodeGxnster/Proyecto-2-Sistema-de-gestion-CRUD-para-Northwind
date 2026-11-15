namespace Northwind.Comun.Configuracion;

/// <summary>
/// Configuracion de acceso a la base de datos Northwind.
/// </summary>
public class BaseDatosSettings
{
    public const string Seccion = "BaseDatos";

    public string CadenaConexion { get; set; } = string.Empty;

    public int TiempoEsperaSegundos { get; set; } = 30;

    public bool ReintentarAnteFallos { get; set; } = true;

    public int MaximoReintentos { get; set; } = 3;
}
