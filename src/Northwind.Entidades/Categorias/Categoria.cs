namespace Northwind.Entidades;

/// <summary>
/// Representa una categoria de productos de Northwind.
/// </summary>
public class Categoria
{
    public int CategoriaId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }
}
