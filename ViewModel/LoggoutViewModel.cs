using System.Windows.Input;
using Microsoft.Maui.Controls;
using PulseTFG.FirebaseService;
using PulseTFG.Pages;

namespace PulseTFG.ViewModel
{
    public class LoggoutViewModel
    {
        private readonly FirebaseAuthService _authService = new FirebaseAuthService();

        public ICommand LogoutCommand => new Command(async () =>
        {
            Shell.Current.FlyoutIsPresented = false;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Cerrar sesión",
                "¿Estás seguro de que quieres cerrar sesión?",
                "Sí", "No");

            if (!confirm)
                return;

            await _authService.SignOutAsync();

            await Application.Current.MainPage.DisplayAlert(
                "Sesión cerrada",
                "Has cerrado sesión correctamente.",
                "OK");

            await Shell.Current.GoToAsync("//LoginPage");
        });

    }
}
