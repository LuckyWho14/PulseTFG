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
        // Navegaci�n a la p�gina de inicio de sesi�n
        Shell.Current.GoToAsync("//LoginPage");
    }

    private void Registro_clicked(object sender, EventArgs e)
    {
        // Navegaci�n a la p�gina de registro
        Shell.Current.GoToAsync("//RegistroPage");
    }
}