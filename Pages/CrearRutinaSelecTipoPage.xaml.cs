using PulseTFG.ViewModel;

namespace PulseTFG.Pages;

public partial class CrearRutinaSelecTipoPage : ContentPage
{
    RoutineCreatorViewModel _vm;

    public CrearRutinaSelecTipoPage()
    {
        InitializeComponent();
        _vm = new RoutineCreatorViewModel();
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync(); 
    }
    private async void OnElegirPredefinidoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//CrearRutinaPredetPage");
    }

    private async void OnElegirPersonalizadoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//CrearRutinaPersPage");
    }

    // Funcion del boton cancelar
    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MisEntrenosPage");
    }
}