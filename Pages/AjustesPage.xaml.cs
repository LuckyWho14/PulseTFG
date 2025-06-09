namespace PulseTFG.Pages;

public partial class AjustesPage : ContentPage
{
    private bool _usuarioInteraccion = false;

    public AjustesPage()
    {
        InitializeComponent();

        // Comprobar si ya se ha guardado un tema antes
        if (!Preferences.ContainsKey("tema_usuario"))
        {
            Preferences.Set("tema_usuario", "oscuro");
            Application.Current.UserAppTheme = AppTheme.Dark;
            ThemePicker.SelectedIndex = 2;
        }
        else
        {
            string tema = Preferences.Get("tema_usuario", "auto");

            switch (tema)
            {
                case "claro":
                    ThemePicker.SelectedIndex = 1;
                    Application.Current.UserAppTheme = AppTheme.Light;
                    break;
                case "oscuro":
                    ThemePicker.SelectedIndex = 2;
                    Application.Current.UserAppTheme = AppTheme.Dark;
                    break;
                default:
                    ThemePicker.SelectedIndex = 0;
                    Application.Current.UserAppTheme = AppTheme.Unspecified;
                    break;
            }
        }

        _usuarioInteraccion = true;
    }

    private async void ThemePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!_usuarioInteraccion)
            return;

        var selectedTheme = ThemePicker.SelectedIndex;
        string temaTexto = "";

        switch (selectedTheme)
        {
            case 0:
                Application.Current.UserAppTheme = AppTheme.Unspecified;
                Preferences.Set("tema_usuario", "auto");
                temaTexto = "automático";
                break;
            case 1:
                Application.Current.UserAppTheme = AppTheme.Light;
                Preferences.Set("tema_usuario", "claro");
                temaTexto = "claro";
                break;
            case 2:
                Application.Current.UserAppTheme = AppTheme.Dark;
                Preferences.Set("tema_usuario", "oscuro");
                temaTexto = "oscuro";
                break;
        }

        await DisplayAlert("Tema actualizado", $"Se ha cambiado al tema {temaTexto}.", "Aceptar");
    }
}
