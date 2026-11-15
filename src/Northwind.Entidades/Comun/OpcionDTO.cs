namespace Northwind.Entidades;

/// <summary>
/// Opcion generica para controles desplegables.
/// </summary>
public class OpcionDTO
{
    public OpcionDTO()
    {
    }

    public OpcionDTO(int valor, string texto)
    {
        Valor = valor;
        Texto = texto;
    }

    public int Valor { get; set; }

    public string Texto { get; set; } = string.Empty;
}
