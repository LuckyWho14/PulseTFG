using PulseTFG.Pages;
using PulseTFG.FirebaseService;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace PulseTFG
{
    public partial class App : Application
    {
        // Instancia de tu servicio de Auth
        readonly FirebaseAuthService _authService = new();
        // Instancia de tu servicio de Firestore
        readonly FirebaseFirestoreService _firestoreService = new();

        // Estas claves deben coincidir con las de FirebaseAuthService
        const string PrefsIdTokenKey = "firebase_id_token";
        const string PrefsRefreshTokenKey = "firebase_refresh_token";
        const string PrefsUserUidKey = "firebase_user_uid";
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
            await Task.Delay(1500);

            var idToken = Preferences.Get(PrefsIdTokenKey, null);
            var uid = Preferences.Get(PrefsUserUidKey, null);

            var esValido =
                !string.IsNullOrEmpty(idToken) &&
                !string.IsNullOrEmpty(uid) &&
                await _authService.TokenEsValidoAsync(idToken);

            if (esValido)
            {
                bool tieneRutinas = await _firestoreService.UsuarioTieneRutinasAsync(uid);

                MainPage = new AppShell();

                if (tieneRutinas)
                    await Shell.Current.GoToAsync("//InicioPage");
                else
                    await Shell.Current.GoToAsync("//CreadorRutinaPage");
            }
            else
            {
                Preferences.Remove(PrefsIdTokenKey);
                Preferences.Remove(PrefsRefreshTokenKey);
                Preferences.Remove(PrefsUserUidKey);

                MainPage = new AppShell();
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}