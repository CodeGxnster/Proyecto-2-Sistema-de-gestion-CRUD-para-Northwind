namespace Northwind.Presentacion;

public class FrmInicio : Form
{
    public FrmInicio()
    {
        Text = "Northwind - Configuracion inicial";
        Size = new Size(620, 280);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(20, 24, 33);
        ForeColor = Color.FromArgb(240, 243, 246);

        var etiqueta = new Label
        {
            Text = "Solucion Northwind configurada.\nLos modulos de la aplicacion se incorporan en las siguientes ramas.",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 11f, FontStyle.Regular)
        };

        Controls.Add(etiqueta);
    }
}
