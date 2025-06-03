namespace PulseTFG.Pages;
>>>>>>> modificacion de la estructura de la solucion; Creación de carpetas y corrección de errores:Pages/HistorialPage.xaml.cs

namespace PulseTFG.Pages
{
    public partial class HistorialPage : ContentPage
    {
        private readonly HistorialViewModel _viewModel;

        public HistorialPage()
        {
            InitializeComponent();
            _viewModel = new HistorialViewModel();
            this.BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Cada vez que la p�gina aparece en pantalla, volvemos a cargar datos
            if (_viewModel.CargarRegistrosCommand.CanExecute(null))
                _viewModel.CargarRegistrosCommand.Execute(null);
        }
    }
}
