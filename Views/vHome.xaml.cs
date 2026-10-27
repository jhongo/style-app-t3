using System.Globalization;
using System.Text.RegularExpressions;
using StyleAppT3.Models;

namespace StyleAppT3.Views;

public partial class vHome : ContentPage
{
    public vHome()
    {
        InitializeComponent();
    }

    private void pkTipo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        txtIdentificacion.Text = string.Empty;

        switch (pkTipo.SelectedItem as string)
        {
            case "CI":
                txtIdentificacion.Placeholder = "10 dígitos";
                txtIdentificacion.Keyboard = Keyboard.Numeric;
                txtIdentificacion.MaxLength = 10;
                break;
            case "RUC":
                txtIdentificacion.Placeholder = "13 dígitos";
                txtIdentificacion.Keyboard = Keyboard.Numeric;
                txtIdentificacion.MaxLength = 13;
                break;
            default:
                txtIdentificacion.Placeholder = "Pasaporte";
                txtIdentificacion.Keyboard = Keyboard.Text;
                txtIdentificacion.MaxLength = int.MaxValue;
                break;
        }
    }

    private async void btnGuardar_Clicked(object? sender, EventArgs e)
    {
        var tipo = pkTipo.SelectedItem as string;
        var identificacion = txtIdentificacion.Text?.Trim() ?? string.Empty;
        var nombres = txtNombres.Text?.Trim() ?? string.Empty;
        var apellidos = txtApellidos.Text?.Trim() ?? string.Empty;
        var correo = txtCorreo.Text?.Trim() ?? string.Empty;
        var salarioTexto = (txtSalario.Text ?? string.Empty).Trim().Replace(',', '.');

        string? error = null;
        if (tipo is null)
            error = "Seleccione el tipo de identificación.";
        else if (tipo == "CI" && !Regex.IsMatch(identificacion, @"^\d{10}$"))
            error = "La CI debe tener exactamente 10 números.";
        else if (tipo == "RUC" && !Regex.IsMatch(identificacion, @"^\d{13}$"))
            error = "El RUC debe tener exactamente 13 números.";
        else if (string.IsNullOrEmpty(identificacion))
            error = "Ingrese el número de pasaporte.";
        else if (string.IsNullOrEmpty(nombres))
            error = "Ingrese los nombres.";
        else if (string.IsNullOrEmpty(apellidos))
            error = "Ingrese los apellidos.";
        else if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            error = "Ingrese un correo válido.";
        else if (!decimal.TryParse(salarioTexto, NumberStyles.Number, CultureInfo.InvariantCulture, out var s) || s < 0)
            error = "Ingrese un salario válido.";

        if (error is not null)
        {
            await DisplayAlertAsync("Validación", error, "OK");
            return;
        }

        var contacto = new Contacto
        {
            TipoIdentificacion = tipo!,
            Identificacion = identificacion,
            Nombres = nombres,
            Apellidos = apellidos,
            Fecha = dpFecha.Date ?? DateTime.Today,
            Correo = correo,
            Salario = decimal.Parse(salarioTexto, NumberStyles.Number, CultureInfo.InvariantCulture)
        };

        await Navigation.PushAsync(new vResumen(contacto));
    }
}
