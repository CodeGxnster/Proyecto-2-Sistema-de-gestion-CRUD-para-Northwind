using Northwind.Entidades;

namespace Northwind.LogicaNegocio.Servicios;

/// <summary>
/// Operaciones de negocio disponibles para los productos.
/// </summary>
public interface IProductoServicio
{
    IReadOnlyList<ProductDTO> Listar(FiltroProductoDTO? filtro = null);

    ProductDTO? ObtenerPorId(int id);

    IReadOnlyList<OpcionDTO> ObtenerCategorias();

    IReadOnlyList<OpcionDTO> ObtenerSuplidores();

    ResultadoOperacion<int> Crear(ProductDTO producto);

    ResultadoOperacion Actualizar(ProductDTO producto);

    ResultadoOperacion Eliminar(int id);
}
