namespace Northwind.LogicaNegocio.Servicios;

/// <summary>
/// Verifica la disponibilidad de la base de datos al iniciar la aplicacion.
/// </summary>
public interface IConexionServicio
{
    bool ProbarConexion(out string mensaje);
}
