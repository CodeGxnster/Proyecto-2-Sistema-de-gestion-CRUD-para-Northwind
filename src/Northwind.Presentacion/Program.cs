using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Comun.Configuracion;
using Northwind.Comun.Registro;
using Northwind.LogicaNegocio.Extensiones;
using Northwind.LogicaNegocio.Servicios;
using Northwind.Presentacion.Formularios;
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

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, args) =>
        {
            Log.Error(args.Exception, "Excepcion no controlada en el hilo de interfaz");
            MostrarError("Ocurrio un error en la interfaz de usuario.", args.Exception.Message);
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception excepcion)
            {
                Log.Fatal(excepcion, "Excepcion no controlada en la aplicacion");
            }
        };

        try
        {
            var cadenaConexion = ConfiguracionAplicacion.ObtenerCadenaConexion(configuracion);
            if (string.IsNullOrWhiteSpace(cadenaConexion))
            {
                MostrarError(
                    "No se encontro la cadena de conexion de Northwind.",
                    "Configure ConnectionStrings:Northwind o BaseDatos:CadenaConexion en appsettings.json.");
                return;
            }

            var reintentar = !bool.TryParse(
                configuracion[$"{BaseDatosSettings.Seccion}:{nameof(BaseDatosSettings.ReintentarAnteFallos)}"],
                out var valorReintentar) || valorReintentar;

            var maximoReintentos = int.TryParse(
                configuracion[$"{BaseDatosSettings.Seccion}:{nameof(BaseDatosSettings.MaximoReintentos)}"],
                out var valorReintentos) ? valorReintentos : 3;

            var servicios = new ServiceCollection();
            servicios.AddSingleton(configuracion);
            servicios.Configure<AplicacionSettings>(configuracion.GetSection(AplicacionSettings.Seccion));
            servicios.Configure<BaseDatosSettings>(configuracion.GetSection(BaseDatosSettings.Seccion));
            servicios.Configure<ReportesSettings>(configuracion.GetSection(ReportesSettings.Seccion));
            servicios.AddSingleton(Log.Logger);

            servicios.AgregarLogicaNegocio(cadenaConexion, reintentar, maximoReintentos);

            servicios.AddTransient<FrmPrincipal>();
            servicios.AddTransient<FrmCategorias>();

            using var proveedor = servicios.BuildServiceProvider();
            using (var alcance = proveedor.CreateScope())
            {
                var conexion = alcance.ServiceProvider.GetRequiredService<IConexionServicio>();
                if (!conexion.ProbarConexion(out var mensajeConexion))
                {
                    var continuar = MessageBox.Show(
                        $"No fue posible conectar con la base de datos Northwind.{Environment.NewLine}{Environment.NewLine}" +
                        $"{mensajeConexion}{Environment.NewLine}{Environment.NewLine}" +
                        "Verifique que el servicio SQL Server Express este iniciado y que la base de datos exista." +
                        $"{Environment.NewLine}{Environment.NewLine}Desea iniciar la aplicacion de todos modos?",
                        "Northwind",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (continuar != DialogResult.Yes)
                    {
                        return;
                    }
                }
            }

            var nombreAplicacion = configuracion[$"{AplicacionSettings.Seccion}:{nameof(AplicacionSettings.Nombre)}"];
            var version = configuracion[$"{AplicacionSettings.Seccion}:{nameof(AplicacionSettings.Version)}"];
            Log.Information("Iniciando {Aplicacion} version {Version}", nombreAplicacion, version);

            Application.Run(proveedor.GetRequiredService<FrmPrincipal>());
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error fatal durante el arranque de la aplicacion");
            MostrarError("Ocurrio un error fatal al iniciar la aplicacion.", ex.Message);
        }
        finally
        {
            Log.Information("Finalizando la aplicacion");
            Log.CloseAndFlush();
        }
    }

    private static void MostrarError(string titulo, string detalle)
    {
        MessageBox.Show(detalle, titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
