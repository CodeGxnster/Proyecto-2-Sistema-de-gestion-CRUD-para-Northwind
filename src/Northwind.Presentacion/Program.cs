using Northwind.Comun.Configuracion;
using Northwind.Comun.Registro;
using Serilog;

namespace Northwind.Presentacion;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        var configuracion = ConfiguracionAplicacion.Cargar();
        Log.Logger = RegistroAplicacion.Crear(configuracion);

        try
        {
            Log.Information(
                "Iniciando {Aplicacion} version {Version} en entorno {Entorno}",
                configuracion[$"{AplicacionSettings.Seccion}:{nameof(AplicacionSettings.Nombre)}"],
                configuracion[$"{AplicacionSettings.Seccion}:{nameof(AplicacionSettings.Version)}"],
                configuracion[$"{AplicacionSettings.Seccion}:{nameof(AplicacionSettings.Entorno)}"]);

            Application.Run(new FrmInicio());
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error fatal durante el arranque de la aplicacion");
            MessageBox.Show(
                $"Ocurrio un error fatal al iniciar la aplicacion: {ex.Message}",
                "Northwind",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
