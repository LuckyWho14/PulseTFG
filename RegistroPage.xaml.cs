namespace PulseTFG;

public partial class RegistroPage : ContentPage
{
	public RegistroPage()
	{
		InitializeComponent();
	}

    private void Olvidada_clicked(object sender, EventArgs e)
    {
        // Navegar a la página de recuperación de contraseña
        Shell.Current.GoToAsync("//ContraseniaOlvidadaPage");
    }

	private void Login_clicked(object sender, EventArgs e)
    {
        // Navegar a la página de inicio de sesión
        Shell.Current.GoToAsync("//LoginPage");
    }
}