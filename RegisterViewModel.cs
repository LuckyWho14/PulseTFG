using System.ComponentModel;
using System.Windows.Input;

namespace PulseTFG;

public class RegisterViewModel : INotifyPropertyChanged
{
    private readonly FirebaseAuthService _authService = new();

    public string Email { get; set; }
    public string Password { get; set; }
    public string NombreCompleto { get; set; }
    public DateTime FechaNacimiento { get; set; } = DateTime.Today;
    public string Sexo { get; set; }
    public double Altura { get; set; }
    public double Peso { get; set; }

    private string errorMessage;
    public string ErrorMessage
    {
        get => errorMessage;
        set
        {
            errorMessage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
        }
    }


    public ICommand RegisterCommand => new Command(async () =>
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(NombreCompleto))
        {
            ErrorMessage = "Por favor, rellena todos los campos obligatorios.";
            return;
        }

        try
        {
            ErrorMessage = "";

            var resultadoRegistro = await _authService.RegisterAsync(Email, Password);

            // Enviar email de verificación
            await _authService.EnviarEmailVerificacionAsync(resultadoRegistro.IdToken);

            var usuario = new Usuario
            {
                Uid = resultadoRegistro.LocalId,
                Email = Email,
                NombreCompleto = NombreCompleto,
                FechaNacimiento = FechaNacimiento,
                Altura = Altura,
                Peso = Peso
            };

            await _authService.GuardarUsuarioAsync(usuario, resultadoRegistro.IdToken);

            await Application.Current.MainPage.DisplayAlert("Éxito", "Usuario registrado correctamente. Te hemos enviado un correo electrónico para que lo confirmes.", "OK");

            await Shell.Current.GoToAsync("///LoginPage");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR]: {ex.Message}");
            ErrorMessage = ex.Message;
        }
    });

    public event PropertyChangedEventHandler PropertyChanged;
}
