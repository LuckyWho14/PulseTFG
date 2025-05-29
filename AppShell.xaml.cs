using PulseTFG.FirebaseService;
using Microsoft.Maui.Storage;
using PulseTFG.ViewModel;
using PulseTFG.Pages;
using Microsoft.Maui.Controls;

namespace PulseTFG
{
    public partial class AppShell : Shell
    {
        readonly FirebaseAuthService _auth = new();


        public AppShell()
        {
            InitializeComponent();

            // Cargar el ViewModel de Logout
            BindingContext = new LoggoutViewModel();

            // Rutas de navegación
            Routing.RegisterRoute(nameof(CrearRutinaCrearEntrenoPage), typeof(CrearRutinaCrearEntrenoPage));
        }


    }
}
