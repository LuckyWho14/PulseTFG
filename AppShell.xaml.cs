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
            Routing.RegisterRoute("CrearRutinaSelectTipoPage", typeof(Pages.CrearRutinaSelecTipoPage));
            Routing.RegisterRoute("CrearRutinaPersPage", typeof(Pages.CrearRutinaPersPage));
            Routing.RegisterRoute("InicioPage", typeof(Pages.InicioPage));
            Routing.RegisterRoute("MisEntrenosPage", typeof(Pages.MisEntrenosPage));


        }


    }
}
