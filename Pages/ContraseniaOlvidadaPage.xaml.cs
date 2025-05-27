namespace PulseTFG.Pages;

using PulseTFG.ViewModel;

public partial class ContraseniaOlvidadaPage : ContentPage
{
    public ContraseniaOlvidadaPage()
    {
        InitializeComponent();

        BindingContext = new RecuperarContrasenaViewModel();

    }

    private void Login_clicked(object sender, EventArgs e)
    {
        // Navigate to the login page
        Shell.Current.GoToAsync("//LoginPage");
    }

    private void Registro_clicked(object sender, EventArgs e)
    {
        // Navigate to the registration page
        Shell.Current.GoToAsync("//RegistroPage");
    }
}