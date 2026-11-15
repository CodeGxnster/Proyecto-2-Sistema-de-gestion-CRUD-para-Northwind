namespace Northwind.Entidades;

/// <summary>
/// Objeto de transferencia de productos con los nombres de sus relaciones.
/// </summary>
public class ProductDTO
{
    public int ProductoId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public int? CategoriaId { get; set; }

    public string NombreCategoria { get; set; } = "Sin categoria";

    public int? SuplidorId { get; set; }

    public string NombreSuplidor { get; set; } = "Sin suplidor";

    public string? CantidadPorUnidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public short UnidadesEnStock { get; set; }

    public short UnidadesEnPedido { get; set; }

    public short NivelReorden { get; set; }

    public bool Descontinuado { get; set; }

    public string EstadoTexto => Descontinuado ? "Descontinuado" : "Activo";
}
