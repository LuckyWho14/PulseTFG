
using System.Collections.ObjectModel;
using PulseTFG.Models;


namespace PulseTFG.Pages;

public partial class InfoEjerPage : ContentPage
{
    public ObservableCollection<Ejercicio> ListaVideos { get; set; }
    public InfoEjerPage()
	{
		InitializeComponent();

        ListaVideos = new ObservableCollection<Ejercicio>
        {
            new Ejercicio { Nombre="Press Banca", 
                VideoId = "dQw4w9WgXcQ", Descripcion = "Ejercicio de fuerza para pecho, hombros y tríceps. Túmbate en un banco con los pies firmes en el suelo, agarra la barra un poco más ancho que los hombros, baja la barra controladamente hasta el pecho y luego empújala hacia arriba hasta extender los brazos sin bloquear los codos. Mantén la espalda ligeramente arqueada y los omóplatos retraídos.", 
                EsFavorito = false },
            new Ejercicio { Nombre="Sentadillas",
                VideoId = "VQRLujxTm3c", Descripcion = "Trabaja piernas y glúteos. Coloca la barra sobre la parte alta de la espalda (no en el cuello), mantén el pecho arriba y la espalda recta, baja controladamente flexionando caderas y rodillas hasta que los muslos estén paralelos al suelo o más abajo si puedes, y sube empujando con fuerza desde los talones.",
                EsFavorito = true }
        };

        this.BindingContext = this;
    }
    private void OnThumbnailTapped(object sender, EventArgs e)
    {
        if (sender is Image image && image.BindingContext is Ejercicio video)
        {
            string videoUrl = $"https://www.youtube.com/embed/{video.VideoId}?autoplay=1&modestbranding=1";
            VideoWebView.Source = videoUrl;
            VideoOverlay.IsVisible = true;
        }
    }

    private void OnOverlayTapped(object sender, EventArgs e)
    {
        // Oculta el overlay y limpia el WebView
        VideoOverlay.IsVisible = false;
        VideoWebView.Source = null;
    }


}