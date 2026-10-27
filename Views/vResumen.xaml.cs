using System.Globalization;
using System.Text;
using StyleAppT3.Models;

namespace StyleAppT3.Views;

public partial class vResumen : ContentPage
{
    private readonly Contacto _contacto;

    public vResumen(Contacto contacto)
    {
        InitializeComponent();
        _contacto = contacto;

        lblTipo.Text = contacto.TipoIdentificacion;
        lblIdentificacion.Text = contacto.Identificacion;
        lblNombres.Text = contacto.Nombres;
        lblApellidos.Text = contacto.Apellidos;
        lblFecha.Text = contacto.Fecha.ToString("dd/MM/yyyy");
        lblCorreo.Text = contacto.Correo;
        lblSalario.Text = FormatoMoneda(contacto.Salario);
        lblAporteTitulo.Text = $"Aporte IESS ({Contacto.PorcentajeIess * 100:0.##}%)";
        lblAporte.Text = FormatoMoneda(contacto.AporteIess);
    }

    private static string FormatoMoneda(decimal valor) =>
        valor.ToString("$#,##0.00", CultureInfo.InvariantCulture);

    private async void btnExportar_Clicked(object? sender, EventArgs e)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== DATOS DEL CONTACTO ===");
        sb.AppendLine($"Tipo de identificación: {_contacto.TipoIdentificacion}");
        sb.AppendLine($"Identificación: {_contacto.Identificacion}");
        sb.AppendLine($"Nombres: {_contacto.Nombres}");
        sb.AppendLine($"Apellidos: {_contacto.Apellidos}");
        sb.AppendLine($"Fecha: {lblFecha.Text}");
        sb.AppendLine($"Correo: {_contacto.Correo}");
        sb.AppendLine($"Salario: {lblSalario.Text}");
        sb.AppendLine($"{lblAporteTitulo.Text}: {lblAporte.Text}");

        try
        {
            var nombreArchivo = $"contacto_{_contacto.Identificacion}_{DateTime.Now:yyyyMMddHHmmss}.txt";
            var ruta = Path.Combine(FileSystem.AppDataDirectory, nombreArchivo);
            await File.WriteAllTextAsync(ruta, sb.ToString());

            await DisplayAlertAsync("Exportado", $"Archivo guardado en:\n{ruta}", "OK");

            // Permite al usuario guardar o compartir el archivo fuera de la app
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Exportar contacto",
                File = new ShareFile(ruta)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"No se pudo exportar: {ex.Message}", "OK");
        }
    }
}
