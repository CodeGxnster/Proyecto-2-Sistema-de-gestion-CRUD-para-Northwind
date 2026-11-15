using Microsoft.Extensions.Options;
using Northwind.Comun.Configuracion;
using Northwind.Comun.Registro;
using Northwind.LogicaNegocio.Servicios;
using Serilog;

namespace Northwind.Presentacion.Formularios;

/// <summary>
/// Reportes analiticos de ventas e inventario con exportacion a CSV y Excel.
/// </summary>
public class FrmReportes : Form
{
    private enum TipoReporte
    {
        TopProductos,
        VentasPorCategoria,
        InventarioSuplidores
    }

    private sealed record ElementoReporte(TipoReporte Valor, string Texto);

    private readonly IReporteServicio _reporteServicio;
    private readonly IExportadorReportes _exportador;
    private readonly ReportesSettings _ajustes;
    private readonly ILogger _logger;

    private ComboBox _cboReporte = null!;
    private DateTimePicker _dtpDesde = null!;
    private DateTimePicker _dtpHasta = null!;
    private Label _lblDesde = null!;
    private Label _lblHasta = null!;
    private Label _lblNota = null!;
    private Button _btnConsultar = null!;
    private Button _btnCsv = null!;
    private Button _btnExcel = null!;
    private DataGridView _gridReporte = null!;
    private Label _lblEstado = null!;

    private string[] _encabezados = Array.Empty<string>();
    private List<IReadOnlyList<object?>> _filas = new();

    public FrmReportes(
        IReporteServicio reporteServicio,
        IExportadorReportes exportador,
        IOptions<ReportesSettings> ajustes,
        ILogger logger)
    {
        _reporteServicio = reporteServicio;
        _exportador = exportador;
        _ajustes = ajustes.Value;
        _logger = logger.ForContext<FrmReportes>();

        Text = "Reportes";
        InicializarComponentes();
        AplicarEstadoFechas();
        Consultar();
    }

    private void InicializarComponentes()
    {
        BackColor = TemaVisual.ColorFondo;
        Font = TemaVisual.FuenteNormal;

        var panelFiltros = new Panel
        {
            Dock = DockStyle.Top,
            Height = 116,
            BackColor = TemaVisual.ColorTarjeta,
            Padding = new Padding(14, 10, 14, 10)
        };

        var lblReporte = TemaVisual.CrearEtiqueta("Reporte:", new Point(14, 22));
        _cboReporte = new ComboBox
        {
            Location = new Point(90, 18),
            Width = 300,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        TemaVisual.AplicarEstiloCombo(_cboReporte);
        _cboReporte.DataSource = new List<ElementoReporte>
        {
            new(TipoReporte.TopProductos, "Productos mas vendidos"),
            new(TipoReporte.VentasPorCategoria, "Ventas por categoria"),
            new(TipoReporte.InventarioSuplidores, "Inventario por suplidor")
        };
        _cboReporte.DisplayMember = nameof(ElementoReporte.Texto);
        _cboReporte.ValueMember = nameof(ElementoReporte.Valor);

        _btnConsultar = TemaVisual.CrearBoton("Consultar", esPrimario: true, ancho: 110, alto: 34);
        _btnConsultar.Location = new Point(404, 15);
        _btnConsultar.Click += (_, _) => Consultar();

        _btnCsv = TemaVisual.CrearBoton("Exportar CSV", esPrimario: false, ancho: 120, alto: 34);
        _btnCsv.Location = new Point(524, 15);
        _btnCsv.Click += (_, _) => Exportar(esExcel: false);

        _btnExcel = TemaVisual.CrearBoton("Exportar Excel", esPrimario: false, ancho: 125, alto: 34);
        _btnExcel.Location = new Point(654, 15);
        _btnExcel.Click += (_, _) => Exportar(esExcel: true);

        _lblDesde = TemaVisual.CrearEtiqueta("Desde:", new Point(14, 70));
        _dtpDesde = new DateTimePicker
        {
            Location = new Point(90, 66),
            Width = 140,
            Format = DateTimePickerFormat.Short
        };
        TemaVisual.AplicarEstiloFecha(_dtpDesde);

        _lblHasta = TemaVisual.CrearEtiqueta("Hasta:", new Point(244, 70));
        _dtpHasta = new DateTimePicker
        {
            Location = new Point(296, 66),
            Width = 140,
            Format = DateTimePickerFormat.Short
        };
        TemaVisual.AplicarEstiloFecha(_dtpHasta);

        _lblNota = new Label
        {
            Text = "El rango de fechas aplica al reporte de ventas por categoria.",
            Location = new Point(450, 69),
            AutoSize = true,
            ForeColor = TemaVisual.ColorTextoSecundario,
            BackColor = Color.Transparent
        };

        _dtpDesde.Value = new DateTime(DateTime.Today.Year, 1, 1);
        _dtpHasta.Value = DateTime.Today;

        panelFiltros.Controls.AddRange(new Control[]
        {
            lblReporte, _cboReporte, _btnConsultar, _btnCsv, _btnExcel,
            _lblDesde, _dtpDesde, _lblHasta, _dtpHasta, _lblNota
        });

        var panelInferior = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 36,
            BackColor = TemaVisual.ColorTarjeta,
            Padding = new Padding(12, 8, 12, 6)
        };

        _lblEstado = new Label
        {
            Text = "Seleccione un reporte y presione Consultar.",
            Dock = DockStyle.Left,
            AutoSize = true,
            ForeColor = TemaVisual.ColorTextoSecundario,
            TextAlign = ContentAlignment.MiddleLeft
        };
        panelInferior.Controls.Add(_lblEstado);

        _gridReporte = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = false };
        TemaVisual.AplicarEstiloGrid(_gridReporte);

        Controls.Add(_gridReporte);
        Controls.Add(panelInferior);
        Controls.Add(panelFiltros);

        _cboReporte.SelectedIndexChanged += (_, _) =>
        {
            AplicarEstadoFechas();
            Consultar();
        };
    }

    private void AplicarEstadoFechas()
    {
        var esVentas = ObtenerTipoReporte() == TipoReporte.VentasPorCategoria;
        _lblDesde.Enabled = esVentas;
        _dtpDesde.Enabled = esVentas;
        _lblHasta.Enabled = esVentas;
        _dtpHasta.Enabled = esVentas;
        _lblNota.Visible = esVentas;
    }

    private TipoReporte ObtenerTipoReporte()
    {
        return _cboReporte.SelectedValue is TipoReporte tipo ? tipo : TipoReporte.TopProductos;
    }

    private void Consultar()
    {
        try
        {
            Cursor = Cursors.WaitCursor;
            _gridReporte.Columns.Clear();
            _gridReporte.DataSource = null;
            _filas = new List<IReadOnlyList<object?>>();

            switch (ObtenerTipoReporte())
            {
                case TipoReporte.TopProductos:
                    ConsultarTopProductos();
                    break;
                case TipoReporte.VentasPorCategoria:
                    ConsultarVentasPorCategoria();
                    break;
                case TipoReporte.InventarioSuplidores:
                    ConsultarInventarioSuplidores();
                    break;
            }

            _gridReporte.ClearSelection();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al consultar el reporte {Reporte}", ObtenerTipoReporte());
            _lblEstado.Text = "No fue posible generar el reporte.";
            MessageBox.Show(
                $"No fue posible generar el reporte.{Environment.NewLine}{Environment.NewLine}{ex.Message}",
                "Reportes",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void ConsultarTopProductos()
    {
        var datos = _reporteServicio.ObtenerTopProductosVendidos();

        AgregarColumna("Producto", nameof(Northwind.Entidades.ReporteTopProductoDTO.NombreProducto), relleno: 180);
        AgregarColumna("Categoria", nameof(Northwind.Entidades.ReporteTopProductoDTO.NombreCategoria), relleno: 120);
        AgregarColumna("Unidades vendidas", nameof(Northwind.Entidades.ReporteTopProductoDTO.UnidadesVendidas), "N0", 80, true);
        AgregarColumna("Total vendido", nameof(Northwind.Entidades.ReporteTopProductoDTO.TotalVendido), "N2", 100, true);

        _gridReporte.DataSource = datos;
        _encabezados = new[] { "Producto", "Categoria", "Unidades vendidas", "Total vendido" };
        _filas = datos
            .Select(d => (IReadOnlyList<object?>)new object?[]
            {
                d.NombreProducto, d.NombreCategoria, d.UnidadesVendidas, d.TotalVendido
            })
            .ToList();

        _lblEstado.Text = $"Productos mas vendidos: {datos.Count} resultados.";
    }

    private void ConsultarVentasPorCategoria()
    {
        if (_dtpDesde.Value.Date > _dtpHasta.Value.Date)
        {
            _gridReporte.DataSource = null;
            _encabezados = Array.Empty<string>();
            _filas.Clear();
            _lblEstado.Text = "La fecha inicial no puede ser mayor que la fecha final.";
            MessageBox.Show(
                "La fecha inicial no puede ser mayor que la fecha final.",
                "Reportes",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var datos = _reporteServicio.ObtenerVentasPorCategoria(_dtpDesde.Value, _dtpHasta.Value);

        AgregarColumna("Categoria", nameof(Northwind.Entidades.ReporteVentasCategoriaDTO.Categoria), relleno: 160);
        AgregarColumna("Unidades vendidas", nameof(Northwind.Entidades.ReporteVentasCategoriaDTO.UnidadesVendidas), "N0", 80, true);
        AgregarColumna("Total ventas", nameof(Northwind.Entidades.ReporteVentasCategoriaDTO.TotalVentas), "N2", 100, true);
        AgregarColumna("% del total", nameof(Northwind.Entidades.ReporteVentasCategoriaDTO.Porcentaje), "P2", 80, true);

        _gridReporte.DataSource = datos;
        _encabezados = new[] { "Categoria", "Unidades vendidas", "Total ventas", "% del total" };
        _filas = datos
            .Select(d => (IReadOnlyList<object?>)new object?[]
            {
                d.Categoria, d.UnidadesVendidas, d.TotalVentas, d.Porcentaje * 100m
            })
            .ToList();

        _lblEstado.Text =
            $"Ventas por categoria del {_dtpDesde.Value:dd/MM/yyyy} al {_dtpHasta.Value:dd/MM/yyyy}: {datos.Count} resultados.";
    }

    private void ConsultarInventarioSuplidores()
    {
        var datos = _reporteServicio.ObtenerSuplidoresMayorValorInventario();

        AgregarColumna("Suplidor", nameof(Northwind.Entidades.ReporteInventarioSuplidorDTO.Suplidor), relleno: 180);
        AgregarColumna("Productos", nameof(Northwind.Entidades.ReporteInventarioSuplidorDTO.CantidadProductos), "N0", 70, true);
        AgregarColumna("Unidades en inventario", nameof(Northwind.Entidades.ReporteInventarioSuplidorDTO.UnidadesEnInventario), "N0", 100, true);
        AgregarColumna("Valor inventario", nameof(Northwind.Entidades.ReporteInventarioSuplidorDTO.ValorInventario), "N2", 110, true);

        _gridReporte.DataSource = datos;
        _encabezados = new[] { "Suplidor", "Productos", "Unidades en inventario", "Valor inventario" };
        _filas = datos
            .Select(d => (IReadOnlyList<object?>)new object?[]
            {
                d.Suplidor, d.CantidadProductos, d.UnidadesEnInventario, d.ValorInventario
            })
            .ToList();

        _lblEstado.Text = $"Suplidores con mayor valor de inventario: {datos.Count} resultados.";
    }

    private void AgregarColumna(
        string encabezado,
        string propiedad,
        string formato = "",
        int relleno = 100,
        bool alinearDerecha = false)
    {
        var columna = new DataGridViewTextBoxColumn
        {
            HeaderText = encabezado,
            DataPropertyName = propiedad,
            FillWeight = relleno,
            SortMode = DataGridViewColumnSortMode.Automatic
        };

        if (!string.IsNullOrWhiteSpace(formato))
        {
            columna.DefaultCellStyle.Format = formato;
        }

        if (alinearDerecha)
        {
            columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        _gridReporte.Columns.Add(columna);
    }

    private void Exportar(bool esExcel)
    {
        if (_filas.Count == 0)
        {
            MessageBox.Show(
                "Primero consulte un reporte con resultados antes de exportar.",
                "Reportes",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        var nombreBase = ObtenerTipoReporte() switch
        {
            TipoReporte.TopProductos => "top-productos",
            TipoReporte.VentasPorCategoria => "ventas-por-categoria",
            _ => "inventario-suplidores"
        };

        using var dialogo = new SaveFileDialog
        {
            Title = "Exportar reporte",
            Filter = esExcel ? "Libro de Excel (*.xlsx)|*.xlsx" : "Archivo CSV (*.csv)|*.csv",
            DefaultExt = esExcel ? "xlsx" : "csv",
            AddExtension = true,
            FileName = $"{nombreBase}-{DateTime.Now:yyyyMMdd-HHmm}",
            InitialDirectory = RutasAplicacion.AsegurarDirectorio(_ajustes.CarpetaExportacion)
        };

        if (dialogo.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            var nombreHoja = ObtenerTipoReporte() switch
            {
                TipoReporte.TopProductos => "Top productos",
                TipoReporte.VentasPorCategoria => "Ventas por categoria",
                _ => "Inventario por suplidor"
            };

            if (esExcel)
            {
                _exportador.ExportarExcel(dialogo.FileName, nombreHoja, _encabezados, _filas);
            }
            else
            {
                _exportador.ExportarCsv(dialogo.FileName, _encabezados, _filas);
            }

            _logger.Information("Reporte exportado en {Ruta}", dialogo.FileName);
            MessageBox.Show(
                $"Reporte exportado correctamente.{Environment.NewLine}{Environment.NewLine}{dialogo.FileName}",
                "Reportes",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al exportar el reporte a {Ruta}", dialogo.FileName);
            MessageBox.Show(
                $"No fue posible exportar el reporte.{Environment.NewLine}{Environment.NewLine}{ex.Message}",
                "Reportes",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
