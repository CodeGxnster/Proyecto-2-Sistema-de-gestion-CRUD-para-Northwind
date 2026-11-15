using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Northwind.AccesoDatos.Contexto;
using Northwind.AccesoDatos.Repositorios;

namespace Northwind.AccesoDatos.Extensiones;

/// <summary>
/// Registro de dependencias de la capa de acceso a datos.
/// </summary>
public static class AccesoDatosExtensiones
{
    /// <summary>
    /// Agrega el contexto de Northwind y los repositorios requeridos por los modulos.
    /// </summary>
    public static IServiceCollection AgregarAccesoDatos(
        this IServiceCollection servicios,
        string cadenaConexion,
        bool reintentarAnteFallos = true,
        int maximoReintentos = 3)
    {
        servicios.AddDbContext<NorthwindDbContext>(opciones =>
        {
            opciones.UseSqlServer(cadenaConexion, sqlServer =>
            {
                if (reintentarAnteFallos)
                {
                    sqlServer.EnableRetryOnFailure(maximoReintentos);
                }
            });
        });

        servicios.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
        servicios.AddScoped<IProductoRepositorio, ProductoRepositorio>();
        servicios.AddScoped<ISuplidorConsultaRepositorio, SuplidorConsultaRepositorio>();
        servicios.AddScoped<IReporteRepositorio, ReporteRepositorio>();

        return servicios;
    }
}
