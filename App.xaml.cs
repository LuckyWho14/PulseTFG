using PulseTFG.Pages;
using PulseTFG.FirebaseService;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System;

namespace PulseTFG
{
    public partial class App : Application
    {
        readonly FirebaseAuthService _authService = new();
        readonly FirebaseFirestoreService _firestoreService = new();

        const string PrefsIdTokenKey = "firebase_id_token";
        const string PrefsRefreshTokenKey = "firebase_refresh_token";
        const string PrefsUserUidKey = "firebase_user_uid";

        public App()
        {
            InitializeComponent();

            // Configuración del tema
            if (!Preferences.ContainsKey("tema_usuario"))
            {
                Preferences.Set("tema_usuario", "oscuro");
                UserAppTheme = AppTheme.Dark;
            }
            else
            {
                string tema = Preferences.Get("tema_usuario", "auto");
                UserAppTheme = tema switch
                {
                    "claro" => AppTheme.Light,
                    "oscuro" => AppTheme.Dark,
                    _ => AppTheme.Unspecified
                };
            }

            // Mostrar pantalla de carga
            MainPage = new SplashPage();

            // Iniciar comprobación de sesión
            _ = InitAppAsync();
        }

        /// <summary>
        /// Guarda el timestamp cuando la app se suspende o se cierra.
        /// </summary>
        protected override void OnSleep()
        {
            Preferences.Set("last_active_timestamp", DateTime.UtcNow.ToString("o"));
        }

        async Task InitAppAsync()
        {
            await Task.Delay(1500); // splash visible al menos 1.5 segundos

            var idToken = Preferences.Get(PrefsIdTokenKey, null);
            var uid = Preferences.Get(PrefsUserUidKey, null);

            if (string.IsNullOrEmpty(idToken) || string.IsNullOrEmpty(uid))
            {
                await IrALoginAsync();
                return;
            }

            // ⏱️ Verificar si se cerró hace menos de 5 minutos
            var lastActiveString = Preferences.Get("last_active_timestamp", null);
            if (!string.IsNullOrEmpty(lastActiveString) &&
                DateTime.TryParse(lastActiveString, null, System.Globalization.DateTimeStyles.RoundtripKind, out var lastActive))
            {
                var tiempoDesdeCierre = DateTime.UtcNow - lastActive;
                var maxInactividad = TimeSpan.FromMinutes(5);

                if (tiempoDesdeCierre <= maxInactividad)
                {
                    // ⚡ Reactivar sesión sin pedir login
                    await EntrarDirectamenteAsync(uid);
                    return;
                }
            }

            // Si ha pasado más de 5 minutos, validar token
            var esValido = await _authService.TokenEsValidoAsync(idToken);
            if (esValido)
            {
                await EntrarDirectamenteAsync(uid);
            }
            else
            {
                await IrALoginAsync();
            }
        }

        async Task EntrarDirectamenteAsync(string uid)
        {
            bool tieneRutinas = await _firestoreService.UsuarioTieneRutinasAsync(uid);
            MainPage = new AppShell();

            if (tieneRutinas)
                await Shell.Current.GoToAsync("//InicioPage");
            else
                await Shell.Current.GoToAsync("//CrearRutinaSelecTipoPage");
        }

        async Task IrALoginAsync()
        {
            Preferences.Remove(PrefsIdTokenKey);
            Preferences.Remove(PrefsRefreshTokenKey);
            Preferences.Remove(PrefsUserUidKey);
            Preferences.Remove("login_timestamp");
            Preferences.Remove("last_active_timestamp");

            MainPage = new AppShell();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
