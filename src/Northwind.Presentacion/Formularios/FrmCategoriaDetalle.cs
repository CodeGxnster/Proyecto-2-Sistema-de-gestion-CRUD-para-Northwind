using Northwind.Entidades;
using Northwind.LogicaNegocio.Servicios;

namespace Northwind.Presentacion.Formularios;

/// <summary>
/// Formulario modal para crear o editar una categoria.
/// </summary>
public class FrmCategoriaDetalle : Form
{
    private readonly ICategoriaServicio _categoriaServicio;
    private readonly int? _categoriaId;
    private CategoryDTO? _categoriaExistente;

    private TextBox _txtNombre = null!;
    private TextBox _txtDescripcion = null!;
    private Label _lblContadorDescripcion = null!;
    private Button _btnGuardar = null!;
    private Button _btnCancelar = null!;
    private ErrorProvider _errorProvider = null!;

    public FrmCategoriaDetalle(ICategoriaServicio categoriaServicio, int? categoriaId = null)
    {
        _categoriaServicio = categoriaServicio;
        _categoriaId = categoriaId;

        InicializarComponentes();
        CargarDatosSiEsEdicion();
    }

    private void InicializarComponentes()
    {
        Text = _categoriaId.HasValue ? "Editar Categoria" : "Nueva Categoria";
        Size = new Size(560, 382);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = TemaVisual.ColorTarjeta;
        ForeColor = TemaVisual.ColorTexto;
        Font = TemaVisual.FuenteNormal;

        _errorProvider = new ErrorProvider(this)
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink
        };

        var lblNombre = new Label
        {
            Text = "Nombre de la categoria (3 - 15 caracteres):",
            Location = new Point(30, 22),
            AutoSize = true,
            ForeColor = TemaVisual.ColorTexto
        };

        _txtNombre = new TextBox
        {
            Location = new Point(30, 48),
            Width = 480,
            MaxLength = 15
        };
        TemaVisual.AplicarEstiloInput(_txtNombre);
        _txtNombre.TextChanged += (_, _) => _errorProvider.SetError(_txtNombre, string.Empty);

        var lblDescripcion = new Label
        {
            Text = "Descripcion (maximo 500 caracteres):",
            Location = new Point(30, 90),
            AutoSize = true,
            ForeColor = TemaVisual.ColorTexto
        };

        _lblContadorDescripcion = new Label
        {
            Text = "0 / 500",
            Location = new Point(445, 90),
            AutoSize = true,
            ForeColor = TemaVisual.ColorTextoSecundario
        };

        _txtDescripcion = new TextBox
        {
            Location = new Point(30, 116),
            Width = 480,
            Height = 120,
            Multiline = true,
            MaxLength = 500,
            ScrollBars = ScrollBars.Vertical
        };
        TemaVisual.AplicarEstiloInput(_txtDescripcion);
        _txtDescripcion.TextChanged += (_, _) =>
        {
            _errorProvider.SetError(_txtDescripcion, string.Empty);
            _lblContadorDescripcion.Text = $"{_txtDescripcion.Text.Length} / 500";
        };

        _btnGuardar = TemaVisual.CrearBoton("Guardar", esPrimario: true, ancho: 110, alto: 36);
        _btnGuardar.Location = new Point(280, 278);
        _btnGuardar.Click += BtnGuardar_Click;

        _btnCancelar = TemaVisual.CrearBoton("Cancelar", esPrimario: false, ancho: 100, alto: 36);
        _btnCancelar.Location = new Point(400, 278);
        _btnCancelar.DialogResult = DialogResult.Cancel;

        Controls.AddRange(new Control[]
        {
            lblNombre,
            _txtNombre,
            lblDescripcion,
            _lblContadorDescripcion,
            _txtDescripcion,
            _btnGuardar,
            _btnCancelar
        });

        AcceptButton = _btnGuardar;
        CancelButton = _btnCancelar;
    }

    private void CargarDatosSiEsEdicion()
    {
        if (!_categoriaId.HasValue)
        {
            return;
        }

        _categoriaExistente = _categoriaServicio.ObtenerPorId(_categoriaId.Value);
        if (_categoriaExistente == null)
        {
            MessageBox.Show(
                "No se encontro la categoria seleccionada.",
                "Categorias",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        _txtNombre.Text = _categoriaExistente.Nombre;
        _txtDescripcion.Text = _categoriaExistente.Descripcion ?? string.Empty;
    }

    private bool ValidarCampos()
    {
        _errorProvider.Clear();
        var esValido = true;

        var nombre = _txtNombre.Text.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            _errorProvider.SetError(_txtNombre, "El nombre es obligatorio.");
            esValido = false;
        }
        else if (nombre.Length < 3 || nombre.Length > 15)
        {
            _errorProvider.SetError(_txtNombre, "El nombre debe tener entre 3 y 15 caracteres.");
            esValido = false;
        }

        if (_txtDescripcion.Text.Trim().Length > 500)
        {
            _errorProvider.SetError(_txtDescripcion, "La descripcion no puede superar los 500 caracteres.");
            esValido = false;
        }

        return esValido;
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (!ValidarCampos())
        {
            return;
        }

        var dto = new CategoryDTO
        {
            CategoriaId = _categoriaId ?? 0,
            Nombre = _txtNombre.Text.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(_txtDescripcion.Text)
                ? null
                : _txtDescripcion.Text.Trim()
        };

        var resultado = _categoriaId.HasValue
            ? _categoriaServicio.Actualizar(dto)
            : _categoriaServicio.Crear(dto);

        if (!resultado.EsExitoso)
        {
            MostrarErroresResultado(resultado);
            return;
        }

        MessageBox.Show(
            resultado.Mensaje,
            "Categorias",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
        Close();
    }

    private void MostrarErroresResultado(ResultadoOperacion resultado)
    {
        foreach (var error in resultado.Errores)
        {
            if (error.Contains("nombre", StringComparison.OrdinalIgnoreCase))
            {
                _errorProvider.SetError(_txtNombre, error);
            }
            else if (error.Contains("descripcion", StringComparison.OrdinalIgnoreCase))
            {
                _errorProvider.SetError(_txtDescripcion, error);
            }
        }

        var detalle = resultado.Errores.Count > 0
            ? string.Join(Environment.NewLine, resultado.Errores)
            : resultado.Mensaje;

        MessageBox.Show(
            detalle,
            "Validacion de categoria",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }
}
