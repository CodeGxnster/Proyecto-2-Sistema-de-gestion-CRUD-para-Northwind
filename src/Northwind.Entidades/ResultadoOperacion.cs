namespace Northwind.Entidades;

/// <summary>
/// Resultado estandar de una operacion de negocio.
/// </summary>
public class ResultadoOperacion
{
    public bool EsExitoso { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public List<string> Errores { get; set; } = new();

    public static ResultadoOperacion Exito(string mensaje = "Operacion completada con exito.")
    {
        return new ResultadoOperacion
        {
            EsExitoso = true,
            Mensaje = mensaje
        };
    }

    public static ResultadoOperacion Falla(string mensaje, IEnumerable<string>? errores = null)
    {
        var resultado = new ResultadoOperacion
        {
            EsExitoso = false,
            Mensaje = mensaje
        };

        if (errores != null)
        {
            resultado.Errores.AddRange(errores);
        }

        return resultado;
    }
}
