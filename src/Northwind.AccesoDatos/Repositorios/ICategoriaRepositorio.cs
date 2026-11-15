using Northwind.Entidades;

namespace Northwind.AccesoDatos.Repositorios;

/// <summary>
/// Contrato de persistencia para las categorias.
/// </summary>
public interface ICategoriaRepositorio
{
    IReadOnlyList<Categoria> ObtenerTodas(string? termino = null);

    Categoria? ObtenerPorId(int id);

    bool ExisteNombre(string nombre, int? excluirId = null);

    int ContarProductos(int categoriaId);

    int Contar();

    Categoria Agregar(Categoria categoria);

    void Actualizar(Categoria categoria);

    void Eliminar(Categoria categoria);
}
