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
        // Navegación a la página de inicio de sesión
        Shell.Current.GoToAsync("//LoginPage");
    }

    private void Registro_clicked(object sender, EventArgs e)
    {
        // Navegación a la página de registro
        Shell.Current.GoToAsync("//RegistroPage");
    }
}