using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Northwind.Comun.Configuracion;
using Northwind.Presentacion.Formularios;

namespace Northwind.Presentacion;

/// <summary>
/// Ventana principal con barra lateral de navegacion entre los modulos.
/// </summary>
public class FrmPrincipal : Form
{
    private readonly IServiceProvider _proveedor;

    private Panel _panelNavegacion = null!;
    private Panel _panelEncabezado = null!;
    private Panel _panelContenedor = null!;
    private Label _lblTituloSeccion = null!;
    private Label _lblSubtitulo = null!;
    private Button? _botonActivo;
    private Form? _formularioActivo;

    public FrmPrincipal(IServiceProvider proveedor, IOptions<AplicacionSettings> opciones)
    {
        _proveedor = proveedor;

        InicializarComponentes(opciones.Value);
    }

    private void InicializarComponentes(AplicacionSettings ajustes)
    {
        Text = $"{ajustes.Nombre} {ajustes.Version}";
        Size = new Size(1220, 740);
        MinimumSize = new Size(1020, 620);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = TemaVisual.ColorFondo;
        Font = TemaVisual.FuenteNormal;

        ConstruirNavegacion();
        ConstruirEncabezado(ajustes);

        _panelContenedor = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = TemaVisual.ColorFondo,
            Padding = new Padding(15)
        };

        Controls.Add(_panelContenedor);
        Controls.Add(_panelEncabezado);
        Controls.Add(_panelNavegacion);

        if (_panelNavegacion.Controls.OfType<Button>().LastOrDefault() is { } botonInicial)
        {
            botonInicial.PerformClick();
        }
    }

    private void ConstruirNavegacion()
    {
        _panelNavegacion = new Panel
        {
            Dock = DockStyle.Left,
            Width = 230,
            BackColor = TemaVisual.ColorTarjeta,
            Padding = new Padding(0)
        };

        var panelLogo = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 65,
            BackColor = Color.FromArgb(16, 20, 28)
        };

        panelLogo.Controls.Add(new Panel
        {
            Dock = DockStyle.Top,
            Height = 1,
            BackColor = TemaVisual.ColorBorde
        });

        panelLogo.Controls.Add(new Label
        {
            Text = "NORTHWIND",
            ForeColor = Color.White,
            Font = TemaVisual.FuenteTitulo,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        });

        _panelNavegacion.Controls.Add(panelLogo);

        var botones = new[]
        {
            CrearBotonMenu("Categorias", (_, _) => AbrirFormulario(_proveedor.GetRequiredService<FrmCategorias>(), _botonCategorias))
        };

        for (var i = botones.Length - 1; i >= 0; i--)
        {
            _panelNavegacion.Controls.Add(botones[i]);
        }
    }

    private Button? _botonCategorias;

    private Button CrearBotonMenu(string texto, EventHandler onClick)
    {
        var boton = new Button
        {
            Text = "   " + texto,
            Dock = DockStyle.Top,
            Height = 48,
            FlatStyle = FlatStyle.Flat,
            ForeColor = TemaVisual.ColorTextoSecundario,
            BackColor = TemaVisual.ColorTarjeta,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = TemaVisual.FuenteNormal,
            Cursor = Cursors.Hand
        };

        boton.FlatAppearance.BorderSize = 0;
        boton.FlatAppearance.MouseOverBackColor = TemaVisual.ColorTarjetaSecundaria;
        boton.Click += onClick;

        if (texto == "Categorias")
        {
            _botonCategorias = boton;
        }

        return boton;
    }

    private void ConstruirEncabezado(AplicacionSettings ajustes)
    {
        _panelEncabezado = new Panel
        {
            Dock = DockStyle.Top,
            Height = 62,
            BackColor = TemaVisual.ColorTarjeta,
            Padding = new Padding(20, 8, 20, 8)
        };

        _panelEncabezado.Controls.Add(new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 1,
            BackColor = TemaVisual.ColorBorde
        });

        _lblTituloSeccion = new Label
        {
            Text = "Categorias",
            Font = TemaVisual.FuenteTitulo,
            ForeColor = Color.White,
            Dock = DockStyle.Top,
            Height = 28,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _lblSubtitulo = new Label
        {
            Text = $"Entorno {ajustes.Entorno}",
            Font = TemaVisual.FuentePequena,
            ForeColor = TemaVisual.ColorTextoSecundario,
            Dock = DockStyle.Top,
            Height = 18,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _panelEncabezado.Controls.Add(_lblSubtitulo);
        _panelEncabezado.Controls.Add(_lblTituloSeccion);
    }

    private void AbrirFormulario(Form formularioHijo, Button? boton)
    {
        if (_formularioActivo != null)
        {
            _formularioActivo.Close();
            _formularioActivo.Dispose();
        }

        if (_botonActivo != null)
        {
            _botonActivo.BackColor = TemaVisual.ColorTarjeta;
            _botonActivo.ForeColor = TemaVisual.ColorTextoSecundario;
            _botonActivo.Font = TemaVisual.FuenteNormal;
        }

        if (boton != null)
        {
            _botonActivo = boton;
            _botonActivo.BackColor = TemaVisual.ColorBotonPrimario;
            _botonActivo.ForeColor = Color.White;
            _botonActivo.Font = TemaVisual.FuenteNormalNegrita;
        }

        _lblTituloSeccion.Text = formularioHijo.Text;

        formularioHijo.TopLevel = false;
        formularioHijo.FormBorderStyle = FormBorderStyle.None;
        formularioHijo.Dock = DockStyle.Fill;

        _panelContenedor.Controls.Clear();
        _panelContenedor.Controls.Add(formularioHijo);
        _formularioActivo = formularioHijo;
        formularioHijo.Show();
    }
}
