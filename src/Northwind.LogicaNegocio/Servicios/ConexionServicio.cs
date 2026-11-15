using Northwind.AccesoDatos.Contexto;
using Serilog;

namespace Northwind.LogicaNegocio.Servicios;

/// <summary>
/// Comprueba la conexion con Northwind sin exponer detalles de EF Core a la interfaz.
/// </summary>
public class ConexionServicio : IConexionServicio
{
    private readonly NorthwindDbContext _contexto;
    private readonly ILogger _logger;

    public ConexionServicio(NorthwindDbContext contexto, ILogger logger)
    {
        _contexto = contexto;
        _logger = logger;
    }

    public bool ProbarConexion(out string mensaje)
    {
        try
        {
            if (_contexto.Database.CanConnect())
            {
                mensaje = "Conexion establecida correctamente.";
                _logger.Information("Conexion con la base de datos Northwind verificada");
                return true;
            }

            mensaje = "No fue posible conectar con la base de datos Northwind.";
            _logger.Warning("No fue posible conectar con Northwind");
            return false;
        }
        catch (Exception ex)
        {
            mensaje = ex.Message;
            _logger.Error(ex, "Fallo al probar la conexion con Northwind");
            return false;
        }
    }
}
