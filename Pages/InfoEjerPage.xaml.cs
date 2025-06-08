
using System.Collections.ObjectModel;
using PulseTFG.Models;
using PulseTFG.ViewModel;


namespace PulseTFG.Pages;

public partial class InfoEjerPage : ContentPage
{
    
    public InfoEjerPage()
	{
		InitializeComponent();

        BindingContext = new GlosarioEjerViewModel();
        
    }

    // Evento que se dispara cuando la página aparece
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