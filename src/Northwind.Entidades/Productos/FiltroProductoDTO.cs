namespace Northwind.Entidades;

/// <summary>
/// Criterios combinables para consultar el catalogo de productos.
/// </summary>
public class FiltroProductoDTO
{
    public int? CategoriaId { get; set; }

    public int? SuplidorId { get; set; }

    public decimal? PrecioMinimo { get; set; }

    public decimal? PrecioMaximo { get; set; }

    public EstadoProducto Estado { get; set; } = EstadoProducto.Todos;

    public string? Texto { get; set; }
}
