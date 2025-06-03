// Origen: PulseTFG/Pages/HistorialPage.xaml.cs
using PulseTFG.ViewModel;

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
            // Cada vez que la página aparece en pantalla, volvemos a cargar datos
            if (_viewModel.CargarRegistrosCommand.CanExecute(null))
                _viewModel.CargarRegistrosCommand.Execute(null);
        }
    }
}
