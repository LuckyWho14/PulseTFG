using PulseTFG.Pages;
using PulseTFG.AuthService;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace PulseTFG
{
    public partial class App : Application
    {
        // Instancia de tu servicio de Auth
        readonly FirebaseAuthService _authService = new();

        // Estas claves deben coincidir con las de FirebaseAuthService
        const string PrefsIdTokenKey = "firebase_id_token";
        const string PrefsRefreshTokenKey = "firebase_refresh_token";

        public App()
        {
            InitializeComponent();

            // 1) Mostrar tu SplashPage XAML (puro UI)
            MainPage = new SplashPage();

            // 2) Arrancar flujo de inicialización
            _ = InitAppAsync();
        }

        async Task InitAppAsync()
        {
            // 2.1) Tiempo para que se vea el splash
            await Task.Delay(1500);

            // 2.2) Leer el idToken de Preferences
            var idToken = Preferences.Get(PrefsIdTokenKey, null);

            // 2.3) Comprobar con Firebase si sigue siendo válido
            var esValido =
                !string.IsNullOrEmpty(idToken) &&
                await _authService.TokenEsValidoAsync(idToken);

            // 2.4) Según el resultado, montar la raíz adecuada
            if (esValido)
            {
                // Sesión activa → mostrar la AppShell con tu contenido protegido
                MainPage = new AppShell();
                await Shell.Current.GoToAsync("//InicioPage");
            }
            else
            {
                // No hay sesión → limpiar cualquier token residual
                Preferences.Remove(PrefsIdTokenKey);
                Preferences.Remove(PrefsRefreshTokenKey);

                // Y mostrar siempre el LoginPage dentro de un NavigationPage
                MainPage = new AppShell();
                await Shell.Current.GoToAsync("//LoginPage");

            }
        }
    }
}
