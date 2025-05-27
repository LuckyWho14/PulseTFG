using PulseTFG.AuthService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PulseTFG.ViewModel
{
    public class RecuperarContrasenaViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseAuthService _authService = new();

        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
            }
        }

        private string mensaje;
        public string Mensaje
        {
            get => mensaje;
            set
            {
                mensaje = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mensaje)));
            }
        }

        public ICommand EnviarCommand => new Command(async () =>
        {
            try
            {
                Mensaje = "";
                await _authService.EnviarCorreoRecuperacionAsync(Email);
                Mensaje = "Correo de recuperación enviado. Revisa tu bandeja.";

                await Application.Current.MainPage.DisplayAlert("Éxito", "Te hemos enviado un correo electrónico oara que restaures tu contraseña.", "OK");

                await Shell.Current.GoToAsync("///LoginPage");
            }
            catch (Exception ex)
            {
                Mensaje = $"Error: {ex.Message}";
            }
        });

        public event PropertyChangedEventHandler PropertyChanged;
    }
}