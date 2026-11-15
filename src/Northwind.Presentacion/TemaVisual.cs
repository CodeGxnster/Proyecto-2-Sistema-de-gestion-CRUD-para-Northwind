using System.Drawing.Drawing2D;

namespace Northwind.Presentacion;

/// <summary>
/// Paleta de colores y estilos visuales compartidos por los formularios.
/// </summary>
public static class TemaVisual
{
    public static readonly Color ColorFondo = Color.FromArgb(20, 24, 33);
    public static readonly Color ColorTarjeta = Color.FromArgb(28, 34, 46);
    public static readonly Color ColorTarjetaSecundaria = Color.FromArgb(36, 44, 58);
    public static readonly Color ColorTexto = Color.FromArgb(240, 243, 246);
    public static readonly Color ColorTextoSecundario = Color.FromArgb(156, 163, 175);
    public static readonly Color ColorBorde = Color.FromArgb(55, 65, 81);
    public static readonly Color ColorBotonPrimario = Color.FromArgb(55, 65, 81);
    public static readonly Color ColorBotonSecundario = Color.FromArgb(36, 44, 58);
    public static readonly Color ColorError = Color.FromArgb(248, 113, 113);

    public static readonly Font FuenteTitulo = new("Segoe UI", 13f, FontStyle.Bold);
    public static readonly Font FuenteSubtitulo = new("Segoe UI", 11f, FontStyle.Bold);
    public static readonly Font FuenteNormal = new("Segoe UI", 9.5f, FontStyle.Regular);
    public static readonly Font FuenteNormalNegrita = new("Segoe UI", 9.5f, FontStyle.Bold);
    public static readonly Font FuentePequena = new("Segoe UI", 8.5f, FontStyle.Regular);

    public static Button CrearBoton(string texto, bool esPrimario = true, int ancho = 130, int alto = 34)
    {
        var color = esPrimario ? ColorBotonPrimario : ColorBotonSecundario;
        return new BotonEstilizado(color)
        {
            Text = texto,
            Size = new Size(ancho, alto)
        };
    }

    public static void AplicarEstiloGrid(DataGridView grid)
    {
        grid.BackgroundColor = ColorTarjeta;
        grid.ForeColor = ColorTexto;
        grid.GridColor = ColorBorde;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.ReadOnly = true;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.EnableHeadersVisualStyles = false;
        grid.Font = FuenteNormal;

        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.ColumnHeadersHeight = 38;
        grid.ColumnHeadersDefaultCellStyle.BackColor = ColorTarjetaSecundaria;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = FuenteNormalNegrita;
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 2, 6, 2);

        grid.DefaultCellStyle.BackColor = ColorTarjeta;
        grid.DefaultCellStyle.ForeColor = ColorTexto;
        grid.DefaultCellStyle.SelectionBackColor = ColorBotonPrimario;
        grid.DefaultCellStyle.SelectionForeColor = Color.White;
        grid.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
        grid.RowTemplate.Height = 32;

        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 30, 42);
        grid.AlternatingRowsDefaultCellStyle.ForeColor = ColorTexto;
        grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = ColorBotonPrimario;
        grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;
    }

    public static void AplicarEstiloInput(TextBox txt)
    {
        txt.BackColor = ColorTarjetaSecundaria;
        txt.ForeColor = ColorTexto;
        txt.BorderStyle = BorderStyle.FixedSingle;
        txt.Font = FuenteNormal;
    }

    public static void AplicarEstiloCombo(ComboBox cbo)
    {
        cbo.BackColor = ColorTarjetaSecundaria;
        cbo.ForeColor = ColorTexto;
        cbo.FlatStyle = FlatStyle.Flat;
        cbo.Font = FuenteNormal;
    }

    public static void AplicarEstiloFecha(DateTimePicker fecha)
    {
        fecha.CalendarMonthBackground = ColorTarjetaSecundaria;
        fecha.CalendarForeColor = ColorTexto;
        fecha.CalendarTitleBackColor = ColorBotonPrimario;
        fecha.CalendarTitleForeColor = Color.White;
        fecha.Font = FuenteNormal;
    }

    public static Label CrearEtiqueta(string texto, Point ubicacion)
    {
        return new Label
        {
            Text = texto,
            Location = ubicacion,
            AutoSize = true,
            ForeColor = ColorTextoSecundario,
            BackColor = Color.Transparent
        };
    }
}
