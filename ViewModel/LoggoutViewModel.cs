using System.Windows.Input;
using Microsoft.Maui.Controls;
using PulseTFG.AuthService;
using PulseTFG.Pages;

namespace PulseTFG.ViewModel
{
    public class LoggoutViewModel
    {
        private readonly FirebaseAuthService _authService = new FirebaseAuthService();

        public ICommand LogoutCommand => new Command(async () =>
        {
            Shell.Current.FlyoutIsPresented = false;

            await _authService.SignOutAsync();

            await Application.Current.MainPage.DisplayAlert(
                "Sesión cerrada",
                "Has cerrado sesión correctamente.",
                "OK");

            await Shell.Current.GoToAsync("//LoginPage");
        });
    }
}
