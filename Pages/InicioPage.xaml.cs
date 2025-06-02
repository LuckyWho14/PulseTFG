using CommunityToolkit.Maui.Views;
using PulseTFG.Models;
using System.Collections.ObjectModel;

namespace PulseTFG.Pages;

public partial class InicioPage : ContentPage
{

    private static bool _seMuestraPopup = false;

    public InicioPage()
	{
        InitializeComponent();

    }

    // Muestra el popup solo una vez al aparecer la página
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (!_seMuestraPopup)
        {
            _seMuestraPopup = true;

            // Muestra el popup de frases motivacionales
            var popup = new MensajePopup();
            this.ShowPopup(popup);
        }
    }
}