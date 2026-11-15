using System.Globalization;
using Northwind.Entidades;
using Northwind.LogicaNegocio.Servicios;

namespace Northwind.Presentacion.Formularios;

/// <summary>
/// Formulario modal para crear o editar un producto.
/// </summary>
public class FrmProductoDetalle : Form
{
    private readonly IProductoServicio _productoServicio;
    private readonly int? _productoId;
    private ProductDTO? _productoExistente;

    private TextBox _txtNombre = null!;
    private TextBox _txtPrecio = null!;
    private TextBox _txtCantidadPorUnidad = null!;
    private ComboBox _cboCategoria = null!;
    private ComboBox _cboSuplidor = null!;
    private NumericUpDown _numStock = null!;
    private NumericUpDown _numPedido = null!;
    private NumericUpDown _numReorden = null!;
    private CheckBox _chkDescontinuado = null!;
    private Button _btnGuardar = null!;
    private Button _btnCancelar = null!;
    private ErrorProvider _errorProvider = null!;

    public FrmProductoDetalle(IProductoServicio productoServicio, int? productoId = null)
    {
        _productoServicio = productoServicio;
        _productoId = productoId;

        InicializarComponentes();
        CargarCombos();
        CargarDatosSiEsEdicion();
    }

    private void InicializarComponentes()
    {
        Text = _productoId.HasValue ? "Editar Producto" : "Nuevo Producto";
        Size = new Size(640, 520);
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

        var lblNombre = CrearEtiqueta("Nombre del producto (maximo 40 caracteres):", new Point(30, 20));
        _txtNombre = new TextBox { Location = new Point(30, 46), Width = 560, MaxLength = 40 };
        TemaVisual.AplicarEstiloInput(_txtNombre);
        _txtNombre.TextChanged += (_, _) => _errorProvider.SetError(_txtNombre, string.Empty);

        var lblPrecio = CrearEtiqueta("Precio unitario:", new Point(30, 88));
        _txtPrecio = new TextBox { Location = new Point(30, 114), Width = 160 };
        TemaVisual.AplicarEstiloInput(_txtPrecio);
        _txtPrecio.TextChanged += (_, _) => _errorProvider.SetError(_txtPrecio, string.Empty);

        var lblCantidad = CrearEtiqueta("Cantidad por unidad (maximo 20 caracteres):", new Point(230, 88));
        _txtCantidadPorUnidad = new TextBox { Location = new Point(230, 114), Width = 360, MaxLength = 20 };
        TemaVisual.AplicarEstiloInput(_txtCantidadPorUnidad);
        _txtCantidadPorUnidad.TextChanged += (_, _) => _errorProvider.SetError(_txtCantidadPorUnidad, string.Empty);

        var lblCategoria = CrearEtiqueta("Categoria:", new Point(30, 156));
        _cboCategoria = new ComboBox
        {
            Location = new Point(30, 182),
            Width = 560,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        TemaVisual.AplicarEstiloCombo(_cboCategoria);
        _cboCategoria.SelectedIndexChanged += (_, _) => _errorProvider.SetError(_cboCategoria, string.Empty);

        var lblSuplidor = CrearEtiqueta("Suplidor:", new Point(30, 224));
        _cboSuplidor = new ComboBox
        {
            Location = new Point(30, 250),
            Width = 560,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        TemaVisual.AplicarEstiloCombo(_cboSuplidor);
        _cboSuplidor.SelectedIndexChanged += (_, _) => _errorProvider.SetError(_cboSuplidor, string.Empty);

        var lblStock = CrearEtiqueta("Unidades en stock:", new Point(30, 292));
        _numStock = CrearNumero(new Point(30, 318));

        var lblPedido = CrearEtiqueta("Unidades en pedido:", new Point(180, 292));
        _numPedido = CrearNumero(new Point(180, 318));

        var lblReorden = CrearEtiqueta("Nivel de reorden:", new Point(330, 292));
        _numReorden = CrearNumero(new Point(330, 318));

        _chkDescontinuado = new CheckBox
        {
            Text = "Descontinuado",
            Location = new Point(480, 318),
            AutoSize = true,
            ForeColor = TemaVisual.ColorTexto,
            FlatStyle = FlatStyle.Flat
        };

        _btnGuardar = TemaVisual.CrearBoton("Guardar", esPrimario: true, ancho: 110, alto: 36);
        _btnGuardar.Location = new Point(360, 400);
        _btnGuardar.Click += BtnGuardar_Click;

        _btnCancelar = TemaVisual.CrearBoton("Cancelar", esPrimario: false, ancho: 100, alto: 36);
        _btnCancelar.Location = new Point(480, 400);
        _btnCancelar.DialogResult = DialogResult.Cancel;

        Controls.AddRange(new Control[]
        {
            lblNombre, _txtNombre,
            lblPrecio, _txtPrecio,
            lblCantidad, _txtCantidadPorUnidad,
            lblCategoria, _cboCategoria,
            lblSuplidor, _cboSuplidor,
            lblStock, _numStock,
            lblPedido, _numPedido,
            lblReorden, _numReorden,
            _chkDescontinuado,
            _btnGuardar, _btnCancelar
        });

        AcceptButton = _btnGuardar;
        CancelButton = _btnCancelar;
    }

    private static Label CrearEtiqueta(string texto, Point ubicacion)
    {
        return new Label
        {
            Text = texto,
            Location = ubicacion,
            AutoSize = true,
            ForeColor = TemaVisual.ColorTexto
        };
    }

    private static NumericUpDown CrearNumero(Point ubicacion)
    {
        return new NumericUpDown
        {
            Location = ubicacion,
            Width = 120,
            Minimum = 0,
            Maximum = short.MaxValue,
            BackColor = TemaVisual.ColorTarjetaSecundaria,
            ForeColor = TemaVisual.ColorTexto,
            BorderStyle = BorderStyle.FixedSingle,
            Font = TemaVisual.FuenteNormal
        };
    }

    private void CargarCombos()
    {
        var categorias = new List<OpcionDTO> { new(0, "-- Seleccione una categoria --") };
        categorias.AddRange(_productoServicio.ObtenerCategorias());
        _cboCategoria.DataSource = categorias;
        _cboCategoria.DisplayMember = nameof(OpcionDTO.Texto);
        _cboCategoria.ValueMember = nameof(OpcionDTO.Valor);

        var suplidores = new List<OpcionDTO> { new(0, "-- Seleccione un suplidor --") };
        suplidores.AddRange(_productoServicio.ObtenerSuplidores());
        _cboSuplidor.DataSource = suplidores;
        _cboSuplidor.DisplayMember = nameof(OpcionDTO.Texto);
        _cboSuplidor.ValueMember = nameof(OpcionDTO.Valor);
    }

    private void CargarDatosSiEsEdicion()
    {
        if (!_productoId.HasValue)
        {
            return;
        }

        _productoExistente = _productoServicio.ObtenerPorId(_productoId.Value);
        if (_productoExistente == null)
        {
            MessageBox.Show(
                "No se encontro el producto seleccionado.",
                "Productos",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        _txtNombre.Text = _productoExistente.Nombre;
        _txtPrecio.Text = _productoExistente.PrecioUnitario.ToString("0.00");
        _txtCantidadPorUnidad.Text = _productoExistente.CantidadPorUnidad ?? string.Empty;
        _cboCategoria.SelectedValue = _productoExistente.CategoriaId ?? 0;
        _cboSuplidor.SelectedValue = _productoExistente.SuplidorId ?? 0;
        _numStock.Value = _productoExistente.UnidadesEnStock;
        _numPedido.Value = _productoExistente.UnidadesEnPedido;
        _numReorden.Value = _productoExistente.NivelReorden;
        _chkDescontinuado.Checked = _productoExistente.Descontinuado;
    }

    private bool ValidarCampos(out decimal precio)
    {
        _errorProvider.Clear();
        var esValido = true;
        precio = 0m;

        var nombre = _txtNombre.Text.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            _errorProvider.SetError(_txtNombre, "El nombre del producto es obligatorio.");
            esValido = false;
        }
        else if (nombre.Length > 40)
        {
            _errorProvider.SetError(_txtNombre, "El nombre del producto no puede superar los 40 caracteres.");
            esValido = false;
        }

        if (!TryParseDecimal(_txtPrecio.Text, out precio) || precio <= 0)
        {
            _errorProvider.SetError(_txtPrecio, "El precio debe ser un numero mayor a 0.");
            esValido = false;
        }
        else if (decimal.Round(precio, 2) != precio)
        {
            _errorProvider.SetError(_txtPrecio, "El precio admite un maximo de 2 decimales.");
            esValido = false;
        }

        if (_txtCantidadPorUnidad.Text.Trim().Length > 20)
        {
            _errorProvider.SetError(_txtCantidadPorUnidad, "La cantidad por unidad no puede superar 20 caracteres.");
            esValido = false;
        }

        if (_cboCategoria.SelectedValue is not int categoriaId || categoriaId <= 0)
        {
            _errorProvider.SetError(_cboCategoria, "Debe seleccionar una categoria.");
            esValido = false;
        }

        if (_cboSuplidor.SelectedValue is not int suplidorId || suplidorId <= 0)
        {
            _errorProvider.SetError(_cboSuplidor, "Debe seleccionar un suplidor.");
            esValido = false;
        }

        return esValido;
    }

    private static bool TryParseDecimal(string texto, out decimal valor)
    {
        return decimal.TryParse(texto.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out valor) ||
               decimal.TryParse(texto.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out valor) ||
               decimal.TryParse(texto.Trim(), NumberStyles.Number, CultureInfo.GetCultureInfo("es-EC"), out valor);
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (!ValidarCampos(out var precio))
        {
            return;
        }

        var dto = new ProductDTO
        {
            ProductoId = _productoId ?? 0,
            Nombre = _txtNombre.Text.Trim(),
            CategoriaId = (int)_cboCategoria.SelectedValue!,
            SuplidorId = (int)_cboSuplidor.SelectedValue!,
            CantidadPorUnidad = string.IsNullOrWhiteSpace(_txtCantidadPorUnidad.Text)
                ? null
                : _txtCantidadPorUnidad.Text.Trim(),
            PrecioUnitario = precio,
            UnidadesEnStock = (short)_numStock.Value,
            UnidadesEnPedido = (short)_numPedido.Value,
            NivelReorden = (short)_numReorden.Value,
            Descontinuado = _chkDescontinuado.Checked
        };

        var resultado = _productoId.HasValue
            ? _productoServicio.Actualizar(dto)
            : _productoServicio.Crear(dto);

        if (!resultado.EsExitoso)
        {
            MostrarErroresResultado(resultado);
            return;
        }

        MessageBox.Show(
            resultado.Mensaje,
            "Productos",
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
            else if (error.Contains("precio", StringComparison.OrdinalIgnoreCase))
            {
                _errorProvider.SetError(_txtPrecio, error);
            }
            else if (error.Contains("cantidad", StringComparison.OrdinalIgnoreCase))
            {
                _errorProvider.SetError(_txtCantidadPorUnidad, error);
            }
            else if (error.Contains("categoria", StringComparison.OrdinalIgnoreCase))
            {
                _errorProvider.SetError(_cboCategoria, error);
            }
            else if (error.Contains("suplidor", StringComparison.OrdinalIgnoreCase))
            {
                _errorProvider.SetError(_cboSuplidor, error);
            }
        }

        var detalle = resultado.Errores.Count > 0
            ? string.Join(Environment.NewLine, resultado.Errores)
            : resultado.Mensaje;

        MessageBox.Show(
            detalle,
            "Validacion de producto",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }
}
