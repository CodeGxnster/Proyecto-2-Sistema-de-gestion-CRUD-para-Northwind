using System.Globalization;
using Northwind.Entidades;
using Northwind.LogicaNegocio.Servicios;

namespace Northwind.Presentacion.Formularios;

/// <summary>
/// Catalogo de productos con filtros combinables en tiempo real.
/// </summary>
public class FrmProductos : Form
{
    private readonly IProductoServicio _productoServicio;

    private DataGridView _gridProductos = null!;
    private TextBox _txtBuscar = null!;
    private ComboBox _cboCategoria = null!;
    private ComboBox _cboSuplidor = null!;
    private TextBox _txtPrecioMin = null!;
    private TextBox _txtPrecioMax = null!;
    private ComboBox _cboEstado = null!;
    private Button _btnNuevo = null!;
    private Button _btnEditar = null!;
    private Button _btnEliminar = null!;
    private Button _btnLimpiar = null!;
    private Label _lblContador = null!;
    private bool _cargandoFiltros;

    public FrmProductos(IProductoServicio productoServicio)
    {
        _productoServicio = productoServicio;

        Text = "Productos";
        InicializarComponentes();
        CargarCombos();
        AplicarFiltros();
    }

    private void InicializarComponentes()
    {
        BackColor = TemaVisual.ColorFondo;
        Font = TemaVisual.FuenteNormal;

        var panelFiltros = new Panel
        {
            Dock = DockStyle.Top,
            Height = 112,
            BackColor = TemaVisual.ColorTarjeta,
            Padding = new Padding(14, 10, 14, 10)
        };

        var lblBuscar = TemaVisual.CrearEtiqueta("Buscar:", new Point(14, 22));
        _txtBuscar = new TextBox { Location = new Point(80, 18), Width = 200 };
        TemaVisual.AplicarEstiloInput(_txtBuscar);
        _txtBuscar.TextChanged += (_, _) => AplicarFiltros();

        _btnNuevo = TemaVisual.CrearBoton("Nuevo Producto", esPrimario: true, ancho: 135, alto: 34);
        _btnNuevo.Location = new Point(296, 15);
        _btnNuevo.Click += BtnNuevo_Click;

        _btnEditar = TemaVisual.CrearBoton("Editar", esPrimario: false, ancho: 95, alto: 34);
        _btnEditar.Location = new Point(441, 15);
        _btnEditar.Click += BtnEditar_Click;

        _btnEliminar = TemaVisual.CrearBoton("Eliminar", esPrimario: false, ancho: 95, alto: 34);
        _btnEliminar.Location = new Point(546, 15);
        _btnEliminar.Click += BtnEliminar_Click;

        _btnLimpiar = TemaVisual.CrearBoton("Limpiar Filtros", esPrimario: false, ancho: 125, alto: 34);
        _btnLimpiar.Location = new Point(651, 15);
        _btnLimpiar.Click += BtnLimpiar_Click;

        var lblCategoria = TemaVisual.CrearEtiqueta("Categoria:", new Point(14, 70));
        _cboCategoria = new ComboBox
        {
            Location = new Point(82, 66),
            Width = 120,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        TemaVisual.AplicarEstiloCombo(_cboCategoria);
        _cboCategoria.SelectedIndexChanged += (_, _) => AplicarFiltros();

        var lblSuplidor = TemaVisual.CrearEtiqueta("Suplidor:", new Point(216, 70));
        _cboSuplidor = new ComboBox
        {
            Location = new Point(286, 66),
            Width = 150,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        TemaVisual.AplicarEstiloCombo(_cboSuplidor);
        _cboSuplidor.SelectedIndexChanged += (_, _) => AplicarFiltros();

        var lblPrecio = TemaVisual.CrearEtiqueta("Precio:", new Point(450, 70));
        _txtPrecioMin = new TextBox { Location = new Point(506, 66), Width = 55 };
        TemaVisual.AplicarEstiloInput(_txtPrecioMin);
        _txtPrecioMin.TextChanged += (_, _) => AplicarFiltros();

        var lblGuion = TemaVisual.CrearEtiqueta("-", new Point(567, 70));
        _txtPrecioMax = new TextBox { Location = new Point(578, 66), Width = 55 };
        TemaVisual.AplicarEstiloInput(_txtPrecioMax);
        _txtPrecioMax.TextChanged += (_, _) => AplicarFiltros();

        var lblEstado = TemaVisual.CrearEtiqueta("Estado:", new Point(646, 70));
        _cboEstado = new ComboBox
        {
            Location = new Point(699, 66),
            Width = 110,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        TemaVisual.AplicarEstiloCombo(_cboEstado);

        _cboEstado.DataSource = new List<ElementoEstado>
        {
            new(EstadoProducto.Todos, "Todos"),
            new(EstadoProducto.Activos, "Activos"),
            new(EstadoProducto.Descontinuados, "Descontinuados")
        };
        _cboEstado.DisplayMember = nameof(ElementoEstado.Texto);
        _cboEstado.ValueMember = nameof(ElementoEstado.Valor);
        _cboEstado.SelectedIndexChanged += (_, _) => AplicarFiltros();

        panelFiltros.Layout += (_, _) =>
        {
            _txtBuscar.Left = lblBuscar.Right + 10;
            _btnNuevo.Left = _txtBuscar.Right + 16;
            _btnEditar.Left = _btnNuevo.Right + 10;
            _btnEliminar.Left = _btnEditar.Right + 10;
            _btnLimpiar.Left = _btnEliminar.Right + 10;

            _cboCategoria.Left = lblCategoria.Right + 8;
            lblSuplidor.Left = _cboCategoria.Right + 14;
            _cboSuplidor.Left = lblSuplidor.Right + 8;
            lblPrecio.Left = _cboSuplidor.Right + 14;
            _txtPrecioMin.Left = lblPrecio.Right + 8;
            lblGuion.Left = _txtPrecioMin.Right + 6;
            _txtPrecioMax.Left = lblGuion.Right + 6;
            lblEstado.Left = _txtPrecioMax.Right + 14;
            _cboEstado.Left = lblEstado.Right + 8;
        };

        panelFiltros.Controls.AddRange(new Control[]
        {
            lblBuscar, _txtBuscar, _btnNuevo, _btnEditar, _btnEliminar, _btnLimpiar,
            lblCategoria, _cboCategoria,
            lblSuplidor, _cboSuplidor,
            lblPrecio, _txtPrecioMin, lblGuion, _txtPrecioMax,
            lblEstado, _cboEstado
        });

        var panelInferior = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 36,
            BackColor = TemaVisual.ColorTarjeta,
            Padding = new Padding(12, 8, 12, 6)
        };

        _lblContador = new Label
        {
            Text = "Total de productos: 0",
            Dock = DockStyle.Left,
            AutoSize = true,
            ForeColor = TemaVisual.ColorTextoSecundario,
            TextAlign = ContentAlignment.MiddleLeft
        };
        panelInferior.Controls.Add(_lblContador);

        _gridProductos = new DataGridView { Dock = DockStyle.Fill };
        TemaVisual.AplicarEstiloGrid(_gridProductos);
        ConfigurarColumnasGrid();

        Controls.Add(_gridProductos);
        Controls.Add(panelInferior);
        Controls.Add(panelFiltros);
    }

    private void ConfigurarColumnasGrid()
    {
        _gridProductos.Columns.Clear();
        _gridProductos.AutoGenerateColumns = false;

        AgregarColumna(nameof(ProductDTO.ProductoId), "ID", 8);
        AgregarColumna(nameof(ProductDTO.Nombre), "Producto", 24);
        AgregarColumna(nameof(ProductDTO.NombreCategoria), "Categoria", 18);
        AgregarColumna(nameof(ProductDTO.NombreSuplidor), "Suplidor", 20);
        AgregarColumna(nameof(ProductDTO.PrecioUnitario), "Precio", 11, "N2", DataGridViewContentAlignment.MiddleRight);
        AgregarColumna(nameof(ProductDTO.UnidadesEnStock), "Stock", 8, null, DataGridViewContentAlignment.MiddleCenter);
        AgregarColumna(nameof(ProductDTO.EstadoTexto), "Estado", 11, null, DataGridViewContentAlignment.MiddleCenter);
    }

    private void AgregarColumna(
        string propiedad,
        string encabezado,
        int peso,
        string? formato = null,
        DataGridViewContentAlignment alineacion = DataGridViewContentAlignment.MiddleLeft)
    {
        var columna = new DataGridViewTextBoxColumn
        {
            DataPropertyName = propiedad,
            HeaderText = encabezado,
            FillWeight = peso,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = alineacion }
        };

        if (!string.IsNullOrWhiteSpace(formato))
        {
            columna.DefaultCellStyle.Format = formato;
        }

        _gridProductos.Columns.Add(columna);
    }

    private void CargarCombos()
    {
        _cargandoFiltros = true;

        try
        {
            var categorias = new List<OpcionDTO> { new(0, "[Todas]") };
            categorias.AddRange(_productoServicio.ObtenerCategorias());
            _cboCategoria.DataSource = categorias;
            _cboCategoria.DisplayMember = nameof(OpcionDTO.Texto);
            _cboCategoria.ValueMember = nameof(OpcionDTO.Valor);

            var suplidores = new List<OpcionDTO> { new(0, "[Todos]") };
            suplidores.AddRange(_productoServicio.ObtenerSuplidores());
            _cboSuplidor.DataSource = suplidores;
            _cboSuplidor.DisplayMember = nameof(OpcionDTO.Texto);
            _cboSuplidor.ValueMember = nameof(OpcionDTO.Valor);

            _cboEstado.SelectedIndex = 0;
        }
        finally
        {
            _cargandoFiltros = false;
        }
    }

    private void AplicarFiltros()
    {
        if (_cargandoFiltros)
        {
            return;
        }

        var filtro = new FiltroProductoDTO
        {
            CategoriaId = ObtenerValorCombo(_cboCategoria),
            SuplidorId = ObtenerValorCombo(_cboSuplidor),
            PrecioMinimo = ObtenerDecimal(_txtPrecioMin),
            PrecioMaximo = ObtenerDecimal(_txtPrecioMax),
            Estado = _cboEstado.SelectedValue is EstadoProducto estado ? estado : EstadoProducto.Todos,
            Texto = _txtBuscar.Text.Trim()
        };

        var productos = _productoServicio.Listar(filtro).ToList();
        _gridProductos.DataSource = null;
        _gridProductos.DataSource = productos;
        _lblContador.Text = $"Total de productos encontrados: {productos.Count}";
    }

    private static int? ObtenerValorCombo(ComboBox combo)
    {
        if (combo.SelectedValue is int valor && valor > 0)
        {
            return valor;
        }

        return null;
    }

    private static decimal? ObtenerDecimal(TextBox texto)
    {
        var valor = texto.Text.Trim();
        if (valor.Length == 0)
        {
            return null;
        }

        if (decimal.TryParse(valor, NumberStyles.Number, CultureInfo.CurrentCulture, out var numero) ||
            decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out numero) ||
            decimal.TryParse(valor, NumberStyles.Number, CultureInfo.GetCultureInfo("es-EC"), out numero))
        {
            return numero > 0 ? numero : null;
        }

        return null;
    }

    private void BtnLimpiar_Click(object? sender, EventArgs e)
    {
        _cargandoFiltros = true;

        try
        {
            _txtBuscar.Clear();
            _txtPrecioMin.Clear();
            _txtPrecioMax.Clear();
            _cboCategoria.SelectedIndex = 0;
            _cboSuplidor.SelectedIndex = 0;
            _cboEstado.SelectedIndex = 0;
        }
        finally
        {
            _cargandoFiltros = false;
        }

        AplicarFiltros();
    }

    private ProductDTO? ObtenerSeleccionado()
    {
        if (_gridProductos.SelectedRows.Count == 0)
        {
            MessageBox.Show(
                "Seleccione un producto de la lista.",
                "Productos",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return null;
        }

        return _gridProductos.SelectedRows[0].DataBoundItem as ProductDTO;
    }

    private void BtnNuevo_Click(object? sender, EventArgs e)
    {
        using var formulario = new FrmProductoDetalle(_productoServicio);
        if (formulario.ShowDialog(this) == DialogResult.OK)
        {
            CargarCombos();
            AplicarFiltros();
        }
    }

    private void BtnEditar_Click(object? sender, EventArgs e)
    {
        var seleccionado = ObtenerSeleccionado();
        if (seleccionado == null)
        {
            return;
        }

        using var formulario = new FrmProductoDetalle(_productoServicio, seleccionado.ProductoId);
        if (formulario.ShowDialog(this) == DialogResult.OK)
        {
            CargarCombos();
            AplicarFiltros();
        }
    }

    private void BtnEliminar_Click(object? sender, EventArgs e)
    {
        var seleccionado = ObtenerSeleccionado();
        if (seleccionado == null)
        {
            return;
        }

        var confirmacion = MessageBox.Show(
            $"Esta seguro de eliminar el producto '{seleccionado.Nombre}'?",
            "Confirmar eliminacion",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmacion != DialogResult.Yes)
        {
            return;
        }

        var resultado = _productoServicio.Eliminar(seleccionado.ProductoId);
        MessageBox.Show(
            resultado.Mensaje,
            resultado.EsExitoso ? "Productos" : "No se pudo eliminar",
            MessageBoxButtons.OK,
            resultado.EsExitoso ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (resultado.EsExitoso)
        {
            AplicarFiltros();
        }
    }

    private sealed record ElementoEstado(EstadoProducto Valor, string Texto);
}
