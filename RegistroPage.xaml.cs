namespace PulseTFG;

public partial class RegistroPage : ContentPage
{
	public RegistroPage()
	{
		InitializeComponent();
	}

    private void Olvidada_clicked(object sender, EventArgs e)
    {
        // Navegar a la p�gina de recuperaci�n de contrase�a
        Shell.Current.GoToAsync("//ContraseniaOlvidadaPage");
    }

	private void Login_clicked(object sender, EventArgs e)
    {
        // Navegar a la p�gina de inicio de sesi�n
        Shell.Current.GoToAsync("//LoginPage");
    }
}