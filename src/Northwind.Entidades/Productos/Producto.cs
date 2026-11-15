namespace Northwind.Entidades;

/// <summary>
/// Producto comercializado por Northwind.
/// </summary>
public class Producto
{
    public int ProductoId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public int? SuplidorId { get; set; }

    public int? CategoriaId { get; set; }

    public string? CantidadPorUnidad { get; set; }

    public decimal? PrecioUnitario { get; set; }

    public short? UnidadesEnStock { get; set; }

    public short? UnidadesEnPedido { get; set; }

    public short? NivelReorden { get; set; }

    public bool Descontinuado { get; set; }

    public Categoria? Categoria { get; set; }

    public Suplidor? Suplidor { get; set; }
}
