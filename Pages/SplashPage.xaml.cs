namespace PulseTFG.Pages;

public partial class SplashPage : ContentPage
{
	public SplashPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Espera visible
        await Task.Delay(2000);

        // Fade-out
        await SplashImage.FadeTo(0, 1000);

        // Cambia a la página principal
        Application.Current.MainPage = new AppShell();
    }
}