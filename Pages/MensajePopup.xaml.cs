// En tu clase MensajePopup.xaml.cs
using CommunityToolkit.Maui.Views;
using PulseTFG.Service;
using System.Diagnostics;

namespace PulseTFG.Pages;

public partial class MensajePopup : Popup
{
    private static string _ultimoMensaje;
    private static DateTime _ultimaSolicitud;
    private const int TIEMPO_ESPERA_MINUTOS = 5; // Espera 5 minutos entre solicitudes

    public MensajePopup()
    {
        InitializeComponent();
        MostrarMensajeAsync();
    }

    private async void MostrarMensajeAsync()
    {
        LabelMensaje.Text = "Cargando consejo motivacional...";

        try
        {
            // Verificar si podemos reutilizar el mensaje anterior
            if (PuedeReutilizarMensaje())
            {
                LabelMensaje.Text = _ultimoMensaje;
                return;
            }

            // Leer la API Key
            var apiKey = await LeerApiKey();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                LabelMensaje.Text = "Configura tu API Key primero";
                return;
            }

            // Obtener nuevo mensaje
            var servicio = new OpenAIService(apiKey);
            var mensaje = await servicio.ObtenerFraseMotivacionalYConsejoAsync();

            // Actualizar caché
            _ultimoMensaje = mensaje;
            _ultimaSolicitud = DateTime.Now;

            LabelMensaje.Text = mensaje;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex}");
            LabelMensaje.Text = "No se pudo obtener el mensaje. Intenta más tarde.";
        }
    }

    private bool PuedeReutilizarMensaje()
    {
        return !string.IsNullOrEmpty(_ultimoMensaje) &&
               (DateTime.Now - _ultimaSolicitud).TotalMinutes < TIEMPO_ESPERA_MINUTOS;
    }

    private async Task<string> LeerApiKey()
    {
        try
        {
            var contenido = await File.ReadAllTextAsync(@"C:\Users\Diego\OpenAI.txt");
            return contenido.Trim().Replace("\n", "").Replace("\r", "");
        }
        catch
        {
            return string.Empty;
        }
    }

    private void OnCerrarClicked(object sender, EventArgs e) => Close();
}