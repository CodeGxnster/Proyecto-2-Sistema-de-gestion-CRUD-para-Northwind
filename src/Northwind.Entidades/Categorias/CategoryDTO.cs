namespace Northwind.Entidades;

/// <summary>
/// Objeto de transferencia utilizado por la capa de presentacion para categorias.
/// </summary>
public class CategoryDTO
{
    public int CategoriaId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public int CantidadProductos { get; set; }
}
