using PulseTFG.FirebaseService;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PulseTFG.ViewModel;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly FirebaseAuthService _authService = new();
    private readonly FirebaseFirestoreService _firestoreService = new();
    const string PrefsUserUidKey = "firebase_user_uid";

    private string email;
    public string Email
    {
        get => email;
        set { email = value; OnPropertyChanged(); }
    }

    private string password;
    public string Password
    {
        get => password;
        set { password = value; OnPropertyChanged(); }
    }

    private string errorMessage;
    public string ErrorMessage
    {
        get => errorMessage;
        set { errorMessage = value; OnPropertyChanged(); }
    }

    public ICommand LoginCommand => new Command(async () =>
    {
        try
        {
            ErrorMessage = string.Empty;
            var token = await _authService.LoginAsync(Email, Password);
            var uid = Preferences.Get(PrefsUserUidKey, null);

            // Aquí guardar token seguro y navegar a siguiente página
            await Application.Current.MainPage.DisplayAlert("Éxito", "Usuario autenticado", "OK");

            var shell = new AppShell();

            // 2) Reemplazas la raíz de la app
            bool tieneRutinas = await _firestoreService.UsuarioTieneRutinasAsync(uid);

            
            Application.Current.MainPage = shell;
            if (tieneRutinas)
                await Shell.Current.GoToAsync("//InicioPage");
            else
                await Shell.Current.GoToAsync("//CrearRutinaSelecTipoPage");

        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    });

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}