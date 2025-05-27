

namespace PulseTFG.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }
    private void PasswordEntry_Completed(object sender, EventArgs e)
    {
        LoginButton.SendClicked();
    }

    private void Registro_clicked(object sender, EventArgs e)
    {
        // Navegar a la página de registro
        Shell.Current.GoToAsync("//RegistroPage");
    }

    private void Olvidaste_clicked(object sender, EventArgs e)
    {
        // Navegar a la página de recuperación de contraseña
        Shell.Current.GoToAsync("//ContraseniaOlvidadaPage");
    }
}