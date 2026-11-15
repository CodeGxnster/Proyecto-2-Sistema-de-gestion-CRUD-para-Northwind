using Northwind.Entidades;

namespace Northwind.LogicaNegocio.Servicios;

/// <summary>
/// Operaciones de negocio disponibles para las categorias.
/// </summary>
public interface ICategoriaServicio
{
    IReadOnlyList<CategoryDTO> Listar(string? termino = null);

    CategoryDTO? ObtenerPorId(int id);

    ResultadoOperacion<int> Crear(CategoryDTO categoria);

    ResultadoOperacion Actualizar(CategoryDTO categoria);

    ResultadoOperacion Eliminar(int id);

    int Contar();
}
