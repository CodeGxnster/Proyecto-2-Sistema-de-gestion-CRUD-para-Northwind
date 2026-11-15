using Northwind.Entidades;

namespace Northwind.AccesoDatos.Repositorios;

/// <summary>
/// Contrato de persistencia para los productos.
/// </summary>
public interface IProductoRepositorio
{
    IReadOnlyList<Producto> ObtenerTodos();

    Producto? ObtenerPorId(int id);

    bool TienePedidos(int productoId);

    Producto Agregar(Producto producto);

    void Actualizar(Producto producto);

    void Eliminar(Producto producto);
}
