using System.Drawing.Drawing2D;

namespace Northwind.Presentacion;

/// <summary>
/// Boton plano con el diseno monocromatico de la aplicacion.
/// </summary>
public class BotonEstilizado : Button
{
    private Color _colorBase;

    public Color ColorBase
    {
        get => _colorBase;
        set
        {
            _colorBase = value;
            BackColor = value;
            FlatAppearance.MouseOverBackColor = ControlPaint.Light(value, 0.15f);
            FlatAppearance.MouseDownBackColor = ControlPaint.Dark(value, 0.15f);
            Invalidate();
        }
    }

    public BotonEstilizado(Color colorFondo)
    {
        _colorBase = colorFondo;
        BackColor = colorFondo;
        ForeColor = Color.White;
        FlatStyle = FlatStyle.Flat;
        Font = TemaVisual.FuenteNormalNegrita;
        Cursor = Cursors.Hand;
        UseVisualStyleBackColor = false;
        FlatAppearance.BorderSize = 0;
        FlatAppearance.MouseOverBackColor = ControlPaint.Light(colorFondo, 0.15f);
        FlatAppearance.MouseDownBackColor = ControlPaint.Dark(colorFondo, 0.15f);
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        var fondo = Enabled ? _colorBase : Color.FromArgb(40, 48, 62);
        using (var brocha = new SolidBrush(fondo))
        {
            pevent.Graphics.FillRectangle(brocha, ClientRectangle);
        }

        var colorTexto = Enabled ? Color.White : ColorTextoSecundario;
        TextRenderer.DrawText(
            pevent.Graphics,
            Text,
            Font,
            ClientRectangle,
            colorTexto,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
    }

    private static Color ColorTextoSecundario => TemaVisual.ColorTextoSecundario;
}
