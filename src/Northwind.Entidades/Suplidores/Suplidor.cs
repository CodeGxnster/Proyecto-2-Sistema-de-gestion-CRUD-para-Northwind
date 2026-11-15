namespace Northwind.Entidades;

/// <summary>
/// Suplidor de Northwind. Se utiliza como catalogo de apoyo para productos.
/// </summary>
public class Suplidor
{
    public int SuplidorId { get; set; }

    public string NombreEmpresa { get; set; } = string.Empty;

    public string? NombreContacto { get; set; }

    public string? Pais { get; set; }
}
