namespace Northwind.Entidades;

/// <summary>
/// Resultado de una operacion que devuelve datos tipados.
/// </summary>
public class ResultadoOperacion<T> : ResultadoOperacion
{
    public T? Datos { get; set; }

    public static ResultadoOperacion<T> ExitoConDatos(T datos, string mensaje = "Operacion completada con exito.")
    {
        return new ResultadoOperacion<T>
        {
            EsExitoso = true,
            Mensaje = mensaje,
            Datos = datos
        };
    }

    public static new ResultadoOperacion<T> Falla(string mensaje, IEnumerable<string>? errores = null)
    {
        var resultado = new ResultadoOperacion<T>
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
