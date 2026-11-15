using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Northwind.AccesoDatos.Extensiones;
using Northwind.Entidades;
using Northwind.LogicaNegocio.Servicios;
using Northwind.LogicaNegocio.Validadores;

namespace Northwind.LogicaNegocio.Extensiones;

/// <summary>
/// Registro de dependencias de la capa de logica de negocio.
/// </summary>
public static class LogicaNegocioExtensiones
{
    /// <summary>
    /// Agrega repositorios, validadores y servicios de negocio de Northwind.
    /// </summary>
    public static IServiceCollection AgregarLogicaNegocio(
        this IServiceCollection servicios,
        string cadenaConexion,
        bool reintentarAnteFallos = true,
        int maximoReintentos = 3)
    {
        servicios.AgregarAccesoDatos(cadenaConexion, reintentarAnteFallos, maximoReintentos);

        servicios.AddScoped<IValidator<CategoryDTO>, CategoriaValidator>();
        servicios.AddScoped<IValidator<ProductDTO>, ProductoValidator>();
        servicios.AddScoped<ICategoriaServicio, CategoriaServicio>();
        servicios.AddScoped<IProductoServicio, ProductoServicio>();
        servicios.AddScoped<IConexionServicio, ConexionServicio>();

        return servicios;
    }
}
