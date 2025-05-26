using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PulseTFG;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly FirebaseAuthService _authService = new();

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

            // Aquí guardar token seguro y navegar a siguiente página
            await Application.Current.MainPage.DisplayAlert("Éxito", "Usuario autenticado", "OK");

            await Shell.Current.GoToAsync("///InicioPage");
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
