using Northwind.Entidades;
using Northwind.LogicaNegocio.Servicios;

namespace Northwind.Presentacion.Formularios;

/// <summary>
/// Listado y administracion de categorias con busqueda en tiempo real.
/// </summary>
public class FrmCategorias : Form
{
    private readonly ICategoriaServicio _categoriaServicio;
    private readonly List<CategoryDTO> _categorias = new();

    private DataGridView _gridCategorias = null!;
    private TextBox _txtBuscar = null!;
    private Button _btnNuevo = null!;
    private Button _btnEditar = null!;
    private Button _btnEliminar = null!;
    private Label _lblContador = null!;

    public FrmCategorias(ICategoriaServicio categoriaServicio)
    {
        _categoriaServicio = categoriaServicio;

        Text = "Categorias";
        InicializarComponentes();
        CargarDatos();
    }

    private void InicializarComponentes()
    {
        BackColor = TemaVisual.ColorFondo;
        Font = TemaVisual.FuenteNormal;

        var panelSuperior = new Panel
        {
            Dock = DockStyle.Top,
            Height = 62,
            BackColor = TemaVisual.ColorTarjeta,
            Padding = new Padding(12, 10, 12, 10)
        };

        var lblBuscar = TemaVisual.CrearEtiqueta("Buscar categoria:", new Point(14, 22));

        _txtBuscar = new TextBox
        {
            Location = new Point(150, 18),
            Width = 240
        };
        TemaVisual.AplicarEstiloInput(_txtBuscar);
        _txtBuscar.TextChanged += (_, _) => FiltrarCategorias();

        _btnNuevo = TemaVisual.CrearBoton("Nueva Categoria", esPrimario: true, ancho: 140, alto: 34);
        _btnNuevo.Location = new Point(410, 15);
        _btnNuevo.Click += BtnNuevo_Click;

        _btnEditar = TemaVisual.CrearBoton("Editar", esPrimario: false, ancho: 95, alto: 34);
        _btnEditar.Location = new Point(560, 15);
        _btnEditar.Click += BtnEditar_Click;

        _btnEliminar = TemaVisual.CrearBoton("Eliminar", esPrimario: false, ancho: 95, alto: 34);
        _btnEliminar.Location = new Point(665, 15);
        _btnEliminar.Click += BtnEliminar_Click;

        panelSuperior.Layout += (_, _) =>
        {
            _txtBuscar.Left = lblBuscar.Right + 12;
            _btnNuevo.Left = _txtBuscar.Right + 16;
            _btnEditar.Left = _btnNuevo.Right + 10;
            _btnEliminar.Left = _btnEditar.Right + 10;
        };

        panelSuperior.Controls.AddRange(new Control[]
        {
            lblBuscar, _txtBuscar, _btnNuevo, _btnEditar, _btnEliminar
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
            Text = "Total de categorias: 0",
            Dock = DockStyle.Left,
            AutoSize = true,
            ForeColor = TemaVisual.ColorTextoSecundario,
            TextAlign = ContentAlignment.MiddleLeft
        };
        panelInferior.Controls.Add(_lblContador);

        _gridCategorias = new DataGridView
        {
            Dock = DockStyle.Fill
        };
        TemaVisual.AplicarEstiloGrid(_gridCategorias);
        ConfigurarColumnasGrid();

        Controls.Add(_gridCategorias);
        Controls.Add(panelInferior);
        Controls.Add(panelSuperior);
    }

    private void ConfigurarColumnasGrid()
    {
        _gridCategorias.Columns.Clear();
        _gridCategorias.AutoGenerateColumns = false;

        _gridCategorias.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(CategoryDTO.CategoriaId),
            HeaderText = "ID",
            FillWeight = 10
        });

        _gridCategorias.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(CategoryDTO.Nombre),
            HeaderText = "Nombre",
            FillWeight = 25
        });

        _gridCategorias.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(CategoryDTO.Descripcion),
            HeaderText = "Descripcion",
            FillWeight = 50
        });

        _gridCategorias.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(CategoryDTO.CantidadProductos),
            HeaderText = "Productos",
            FillWeight = 15,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            }
        });
    }

    private void CargarDatos()
    {
        _categorias.Clear();
        _categorias.AddRange(_categoriaServicio.Listar());
        _gridCategorias.DataSource = null;
        _gridCategorias.DataSource = _categorias.ToList();
        ActualizarContador(_categorias.Count);
    }

    private void FiltrarCategorias()
    {
        var termino = _txtBuscar.Text.Trim();
        var filtradas = _categoriaServicio.Listar(termino).ToList();

        _gridCategorias.DataSource = null;
        _gridCategorias.DataSource = filtradas;
        ActualizarContador(filtradas.Count);
    }

    private void ActualizarContador(int cantidad)
    {
        _lblContador.Text = $"Total de categorias: {cantidad}";
    }

    private CategoryDTO? ObtenerSeleccionada()
    {
        if (_gridCategorias.SelectedRows.Count == 0)
        {
            MessageBox.Show(
                "Seleccione una categoria de la lista.",
                "Categorias",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return null;
        }

        return _gridCategorias.SelectedRows[0].DataBoundItem as CategoryDTO;
    }

    private void BtnNuevo_Click(object? sender, EventArgs e)
    {
        using var formulario = new FrmCategoriaDetalle(_categoriaServicio);
        if (formulario.ShowDialog(this) == DialogResult.OK)
        {
            CargarDatos();
        }
    }

    private void BtnEditar_Click(object? sender, EventArgs e)
    {
        var seleccionada = ObtenerSeleccionada();
        if (seleccionada == null)
        {
            return;
        }

        using var formulario = new FrmCategoriaDetalle(_categoriaServicio, seleccionada.CategoriaId);
        if (formulario.ShowDialog(this) == DialogResult.OK)
        {
            CargarDatos();
        }
    }

    private void BtnEliminar_Click(object? sender, EventArgs e)
    {
        var seleccionada = ObtenerSeleccionada();
        if (seleccionada == null)
        {
            return;
        }

        var confirmacion = MessageBox.Show(
            $"Esta seguro de eliminar la categoria '{seleccionada.Nombre}'?",
            "Confirmar eliminacion",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmacion != DialogResult.Yes)
        {
            return;
        }

        var resultado = _categoriaServicio.Eliminar(seleccionada.CategoriaId);
        MessageBox.Show(
            resultado.Mensaje,
            resultado.EsExitoso ? "Categorias" : "No se pudo eliminar",
            MessageBoxButtons.OK,
            resultado.EsExitoso ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (resultado.EsExitoso)
        {
            CargarDatos();
        }
    }
}
