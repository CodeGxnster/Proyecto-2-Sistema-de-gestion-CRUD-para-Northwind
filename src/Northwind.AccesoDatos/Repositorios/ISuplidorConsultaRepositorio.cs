using Northwind.Entidades;

namespace Northwind.AccesoDatos.Repositorios;

/// <summary>
/// Consulta de solo lectura del catalogo de suplidores para los filtros de productos.
/// </summary>
public interface ISuplidorConsultaRepositorio
{
    IReadOnlyList<Suplidor> ObtenerTodos();

    Suplidor? ObtenerPorId(int id);
}
